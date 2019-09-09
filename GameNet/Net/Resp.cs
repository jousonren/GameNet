using UnityEngine;
using System.Collections;

/// <summary>
/// 基础的返回消息，所有服务器返回的消息继承此类
/// </summary>
public class Resp {
    /// <summary>
    /// 子类必须实现协议获取的方法，返回此请求所对应的协议
    /// </summary>
    /// <returns></returns>
	public virtual int GetProtocol() {
        Debug.LogError("can't get Protocol");
        return 0;
    }

    /// <summary>
    /// 反序列化方法
    /// </summary>
    /// <param name="reader"></param>
	public Resp Deserialize(DataStream reader) {
        string tem = reader.ReadString16();
        //Debug.Log(tem);
        Resp resp = (Resp)JsonUtility.FromJson(tem, GetType());
        return resp;
    }
}
