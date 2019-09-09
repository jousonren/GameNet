# GameNet
基于TCP的网络游戏通信框架，处理了粘包和半包问题。


# socket数据包协议

	Sockte参数:  AddressFamily.InterNetwork

 		               SocketType.Stream

				ProtocolType.Tcp

	Socket数据包：

			数据包=数据包长度(固定长度4byte，UInt32类型)+协议编号(固定长度4byte，Int32类型)+数据

			 数据：编码 json/UTF-8

			发送：在数据头4位装入数据长度，以便于进行粘包的处理（前4位存入一个uint32类型的数据表示							后面的数据转换成byte[]时的长度）。对数据进行序列化，转为2进制发送。

			接收：根据数据头4位装入的数据长度判断缓冲区是否接收到了一份完整的数据包，若接收到将该数据取出根据协议进行解析。



# 使用说明

#### 发送网络协议

​	

```c#
    TestReq req = new TestReq("");
    req.Send();
```

​		说明：TestReq为继承Request的子类，所有网络请求需要使用的类都在ProtocolInstance脚本中，实例化Request子类后调用Send方法会将子类数据序列化后发送非游戏服务器。

#### 接收网络请求

##### 1、注册网络监听

``` c#
        ProtoManager.Instance.AddRespDelegate(NetProtocols.Test,Response);
```

​		说明：服务器发送的数据需要注册监听，NetProtocols枚举里包含所有的网络请求类型。Response为回调函数，需要一个Resp参数。

##### 2、网络数据解析

``` c#
	void Response(Resp r){
        Debug.Log( " receive server msg : " + r.GetProtocol());
        if (r.GetProtocol() == NetProtocols.Test){
            Debug.Log(NetProtocols.Test.ToString());
            TestResp resp = (TestResp)r;
            Debug.Log(resp.msg);
        }
	}
```

​		说明：网络解析时需要将Resp参数根据需要转换成对应类型。

##### 3、删除网络监听

``` 
    ProtoManager.Instance.DelRespDelegate(NetProtocols.Test,Response);
```

​		说明：脚本在失效时需要移除监听，防止下次进入时会发生两次回调。
