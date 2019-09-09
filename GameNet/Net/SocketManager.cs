using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

/// <summary>
/// socket管理器
/// </summary>
public class SocketManager : MonoBehaviour {
    /// <summary>
    /// 数据缓存区
    /// </summary>
	private DataBuffer dataBuffer = new DataBuffer();
    private static SocketManager instance;
    public delegate void ConnectResultCallback(ConnectResult connectResult);
    /// <summary>
    /// 连接状态
    /// </summary>
	ConnectResultCallback connectResultDelegate = null;
    Queue<byte[]> dataQueue = new Queue<byte[]>();
    Queue<Request> sendDataQueue = new Queue<Request>();
    private Socket socket;
    private Thread receiveMsgThread;
    private Thread sendMsgThread;
    private bool receiveMsgFlag;
    private bool sendMsgFlag;
    bool isStopReceive = true;
    /// <summary>
    /// 单利
    /// </summary>
    /// <returns></returns>
	public static SocketManager Instance() {
        return instance;
    }

    void Awake() {
        instance = this;
    }
    private void Update() {
        int length;
        length = dataQueue.Count;
        if (length > 0) {
            for (int i = 0; i < length; i++) {
                ProtoManager.Instance.TryDeserialize(dataQueue.Dequeue());
            }
        }
    }
    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <param name="serverIp">IP</param>
    /// <param name="serverPort">端口</param>
    /// <param name="connectCallback">连接成功的回调</param>
    /// <param name="connectFailedCallback">连接失败的回调</param>
    public void ConnectServer(string serverIp, int serverPort, ConnectResultCallback connectCallback) {
        RegisterResp.RegisterAll();
        connectResultDelegate = connectCallback;
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress address = IPAddress.Parse(serverIp);
        IPEndPoint endpoint = new IPEndPoint(address, serverPort);
        socket.NoDelay = true;
        IAsyncResult result = socket.BeginConnect(endpoint, new AsyncCallback(ConnectedCallback), socket);
        //超时监测，当连接超过5秒还没成功表示超时  
        bool success = result.AsyncWaitHandle.WaitOne(5000, true);
        if (!success) {
            Closed();
            connectResultDelegate?.Invoke(ConnectResult.fail);
        } else {
            isStopReceive = false;
            receiveMsgThread = new Thread(new ThreadStart(ReceiveMessage));
            receiveMsgThread.IsBackground = true;
            receiveMsgFlag = false;
            receiveMsgThread.Start();
            sendMsgThread = new Thread(new ThreadStart(SendCheck));
            sendMsgThread.IsBackground = true;
            sendMsgFlag = false;
            sendMsgThread.Start();
        }
    }
    /// <summary>
    /// 连接完成后的回调
    /// </summary>
    /// <param name="asyncConnect"></param>
	private void ConnectedCallback(IAsyncResult asyncConnect) {
        if (!socket.Connected) {
            connectResultDelegate?.Invoke(ConnectResult.fail);
            return;
        }
        connectResultDelegate?.Invoke(ConnectResult.success);
    }
    /// <summary>
    /// 接收服务器数据
    /// </summary>
	private void ReceiveMessage() {
        dataBuffer.Reset();
        while (!isStopReceive) {
            if (receiveMsgFlag) {
                receiveMsgFlag = false;
                return;
            }
            if (!socket.Connected) {
                Debug.Log("连接服务器失败");
                socket.Close();
                break;
            }
            try {
                byte[] bytes = new byte[4096];
                int i = socket.Receive(bytes);
                if (i <= 0) {
                    socket.Close();
                    break;
                }
                dataBuffer.PushData(bytes, i);
                while (dataBuffer.IsFinished()) {
                    lock (dataQueue) {
                        dataQueue.Enqueue(dataBuffer.ReceiveData);
                    }
                    dataBuffer.RemoveFromHead();
                }
            } catch (Exception e) {
                Debug.Log("Failed to clientSocket error." + e);
                socket.Close();
                break;
            }
        }
    }
    /// <summary>
    /// 关闭Socket
    /// </summary>
    public void Closed() {
        isStopReceive = true;
        if (socket != null && socket.Connected) {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            receiveMsgFlag = true;
            while ((receiveMsgThread.ThreadState !=ThreadState.Stopped) && (receiveMsgThread.ThreadState != ThreadState.Aborted)) {
                Thread.Sleep(10);
            }
            sendMsgFlag = true;
            while ((sendMsgThread.ThreadState != ThreadState.Stopped) && (sendMsgThread.ThreadState != ThreadState.Aborted)) {
                Thread.Sleep(10);
            }
        }
        socket = null;
    }
    /// <summary>
    /// 当前是否处于连接状态
    /// </summary>
    /// <returns></returns>
    public bool IsConnect() {
        return socket != null && socket.Connected;
    }
    /// <summary>
    /// 向服务器发送信息
    /// </summary>
    /// <param name="req"></param>
	public void SendMessage(Request req) {
        lock (sendDataQueue) {
            sendDataQueue.Enqueue(req);
        }
    }
    private void SendCheck() {
        while (true) {
            if (receiveMsgFlag) {
                receiveMsgFlag = false;
                return;
            }
            int length = sendDataQueue.Count;
            if (length > 0) {
                for (int i = 0; i < length; i++) {
                    Send();
                }
            }
        }
    }
    /// <summary>
    /// 从发送队列中取出要发送的数据，然后对数据进行序列化，转为2进制数据，然后在数据最前端加上数据的长度，以便于服务器进行粘包的处理。
    /// </summary>
    private void Send() {
        if (socket == null) {
            return;
        }
        if (!socket.Connected) {
            Closed();
            return;
        }
        try {
            Request req;
            lock (sendDataQueue) {
                req = sendDataQueue.Dequeue();
            }
            DataStream bufferWriter = new DataStream(true);
            req.Serialize(bufferWriter, req);
            //msg：协议号+ 参数
            byte[] msg = bufferWriter.ToByteArray();
            byte[] buffer = new byte[msg.Length + 4];
            DataStream writer = new DataStream(buffer, true);
            writer.WriteInt32((uint)msg.Length - 4);//增加数据长度
            writer.WriteRaw(msg);
            byte[] data = writer.ToByteArray();
            IAsyncResult asyncSend = socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            //bool success = asyncSend.AsyncWaitHandle.WaitOne(5000, true);
            //if (!success) {
            //    Closed();
            //}
        } catch (Exception e) {
            Debug.Log("send error : " + e.ToString());
        }
    }
    /// <summary>
    /// 发送信息完成后的回调
    /// </summary>
    /// <param name="asyncConnect"></param>
	private void SendCallback(IAsyncResult asyncConnect) {
    }

    void OnDestroy() {
        Closed();
    }
}
