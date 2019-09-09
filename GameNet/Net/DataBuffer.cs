using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// 管理接收到的数据，并处理粘包和丢包的情况(根据数据长度对数据进行裁切)
/// </summary>
public class DataBuffer
{
    /// <summary>
    /// 接收的数据缓存
    /// </summary>
    public byte[] ReceiveDataCache;
    /// <summary>
    /// 缓冲区首部一个完整的数据包
    /// </summary>
    public byte[] ReceiveData;
    /// <summary>
    /// 缓冲区的尾部索引
    /// </summary>
    private int EndPos = -1;
    /// <summary>
    /// 缓冲区首部完整数据的长度(从数据首部4byte的数据中读取长度)
    /// </summary>
    private int packageLength;
    /// <summary>
    /// 保存收到的服务器数据，放入数据缓存中
    /// </summary>
    /// <param name="data"></param>
    /// <param name="length"></param>
    public void PushData(byte[] data, int length)
    {
        if (ReceiveDataCache == null) {
            ReceiveDataCache = new byte[length];
        }
        if (Count + length > Capacity)
        {
            byte[] newArr = new byte[Count + length];
            ReceiveDataCache.CopyTo(newArr, 0);
            ReceiveDataCache = newArr;
        }
        Array.Copy(data, 0, ReceiveDataCache, EndPos + 1, length);
        EndPos += length;
    }
    /// <summary>
    /// 判断接收到的数据是否完整，服务器在数据头4位装入数据长度，如果接收到完整数据，则将数据复制出一份供外部获取。
    /// </summary>
    /// <returns></returns>
    public bool IsFinished()
    {
        if (Count == 0)
        {
            return false;
        }
        if (Count >= 4)
        {
            DataStream reader = new DataStream(ReceiveDataCache, true);
            packageLength = (int)reader.ReadInt32()+4;
            if (packageLength > 0)
            {
                if (Count - 4 >= packageLength)
                {
                    ReceiveData = new byte[packageLength];
                    Array.Copy(ReceiveDataCache, 4, ReceiveData, 0, packageLength);
                    return true;
                }
                return false;
            }
            return false;
        }
        return false;
    }
    /// <summary>
    /// 重置缓冲区索引
    /// </summary>
    public void Reset()
    {
        EndPos = -1;
    }
    /// <summary>
    /// 如果接收到了完整的数据，则调用此方法将完整数据从数据缓存移除
    /// </summary>
    public void RemoveFromHead()
    {
        int countToRemove = packageLength + 4;
        if (countToRemove > 0 &&Count - countToRemove > 0)
        {
            Array.Copy(ReceiveDataCache, countToRemove, ReceiveDataCache, 0, Count - countToRemove);
        }
        EndPos -= countToRemove;
    }

    /// <summary>
    /// 缓冲区的容量
    /// </summary>
    private int Capacity
    {
        get
        {
            return ReceiveDataCache != null ? ReceiveDataCache.Length : 0;
        }
    }
    /// <summary>
    /// 当前缓冲区的数据长度
    /// </summary>
    private int Count
    {
        get
        {
            return EndPos + 1;
        }
    }
}