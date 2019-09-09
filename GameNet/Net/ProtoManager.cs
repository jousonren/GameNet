using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 用来管理所有协议，进行消息的解析以及分发
/// </summary>
public class ProtoManager
{
	private static ProtoManager instance;
    /// <summary>
    /// 管理所有的协议
    /// </summary>
	private Dictionary<int, Func<DataStream, Resp>> mProtocolMapping;
	public delegate void responseDelegate(Resp resp);
    /// <summary>
    /// 管理所有的回调
    /// </summary>
	private Dictionary<int, List<responseDelegate>> mDelegateMapping;
    /// <summary>
    /// 单利
    /// </summary>
    public static ProtoManager Instance {
        get {
            if (instance == null) {
                instance = new ProtoManager();
            }
            return instance;
        }
    }

    private ProtoManager()
	{
		mProtocolMapping = new Dictionary<int, Func<DataStream, Resp>>();
		mDelegateMapping = new Dictionary<int, List<responseDelegate>>();
	}
    /// <summary>
    /// 所有Resp需要在这里注册，以便接收到数据时进行解析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="protocol"></param>
	public void AddProtocol<T>(int protocol) where T: Resp, new()
	{
		if (mProtocolMapping.ContainsKey(protocol))
		{
			mProtocolMapping.Remove(protocol);
		}

		mProtocolMapping.Add(protocol, 
			(stream) => {
				T data = new T();
				return data.Deserialize(stream);
			});
	}

    /// <summary>
    /// 注册相应协议的代理，在接收到服务器数据后，会调用这里注册过的相应代理
    /// </summary>
    /// <param name="protocol">Protocol.</param>
    /// <param name="d">D.</param>
    public void AddRespDelegate(int protocol,responseDelegate d){
		List<responseDelegate>  dels;
		if (mDelegateMapping.ContainsKey(protocol))
		{
			dels = mDelegateMapping[protocol];
			for(int i = 0 ; i < dels.Count ; i ++){
				if(dels[i] == d){
					return;
				}
			}
		}else{
			dels = new List<responseDelegate>();
			mDelegateMapping.Add(protocol,dels);
		}
		dels.Add(d);
	}
    /// <summary>
    /// 删除注册过的代理
    /// </summary>
    /// <param name="protocol"></param>
    /// <param name="d"></param>
	public void DelRespDelegate(int protocol,responseDelegate d){
		if (mDelegateMapping.ContainsKey(protocol))
		{
			List<responseDelegate> dels = mDelegateMapping[protocol];
			dels.Remove(d);
		}
	}
    /// <summary>
    /// 接收到服务器数据后，服务器会调用此方法对数据进行解析并调用相应的代理方法 
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
	public Resp TryDeserialize(byte[] buffer)
	{
        DataStream stream = new DataStream(buffer, true);
		int protocol = stream.ReadSInt32();
		Resp resp = null;
		if (mProtocolMapping.ContainsKey(protocol))
		{
            //通过委托解析数据
			resp = mProtocolMapping[protocol](stream);
			if(resp != null){
				if(mDelegateMapping.ContainsKey(protocol)){
					List<responseDelegate> dels = mDelegateMapping[protocol];
					for(int i = 0 ; i < dels.Count ; i ++){
                        //通过委托调用事件
						dels[i](resp);
					}
				}
			}
		}else{
            Debug.Log("no register protocol : " + protocol +"!please reg to RegisterResp.");
		}
		return resp;
	}
}


