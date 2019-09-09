using UnityEngine;
using System.Collections;
/// <summary>
/// 基础的请求消息，所有要发送的请求消息继承此类
/// </summary>
public class Request {
    /// <summary>
    /// 子类必须实现协议获取的方法，返回此请求所对应的协议
    /// </summary>
    /// <returns></returns>
	public virtual int GetProtocol(){
        Debug.LogError("can't get Protocol");
		return -1;
	}
    /// <summary>
    /// 序列化方法 
    /// </summary>
    /// <param name="writer"></param>
	public void Serialize(DataStream writer, Request request)
	{
		writer.WriteSInt32(GetProtocol());
        writer.WriteString16(JsonUtility.ToJson(request));
    }

	public void Send(){
            SocketManager.Instance().SendMessage(this);
	}
}
