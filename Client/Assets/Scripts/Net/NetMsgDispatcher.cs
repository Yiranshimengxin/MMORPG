using Pb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public delegate ProtoBuf.IExtensible RpcReqHandler(ProtoBuf.IExtensible rpcReq);
public delegate void RpcRspHandler(byte[] rpcRsp);

//send and receive game play msg from server
//消息分发的逻辑处理类
public class NetMsgDispatcher
{
    //处理服务器推送的协议定义
    public delegate void OnMessageHandler(byte[] rpcRsp);
    //每个名字对应一个回调函数
    private Dictionary<string, OnMessageHandler> mCallbacks = new Dictionary<string, OnMessageHandler>();
    static NetMsgDispatcher _instance = null;

    //处理客户端发送的的RPC的定义
    private static Dictionary<long, RpcRspHandler> rpcRspHandlerDict;
    //定义每次请求的唯一ID，每发送一个PRC，就加1
    private int session = 0;
    private int maxSession = int.MaxValue;


    private NetMsgDispatcher() { }
    public static NetMsgDispatcher GetInstance()
    {
        if (_instance != null)
        {
            return _instance;
        }
        _instance = new NetMsgDispatcher();
        return _instance;
    }

    //初始化函数
    public void Init()
    {
        rpcRspHandlerDict = new Dictionary<long, RpcRspHandler>();
        //给NetworkManager里的OnReceiveMsgCallBack()赋值
        NetworkManager.GetInstance().OnReceiveMsgCallBack += OnReceiveMsgFromNet;
    }

    //给PRC注册一个回调
    private static void AddHandler(long session, RpcRspHandler rpcRspHandler)
    {
        rpcRspHandlerDict.Add(session, rpcRspHandler);
    }

    //删除一个PRC调用的回调，执行完一个PRC的时候删除
    private static void RemoveHandler(long session)
    {
        if (rpcRspHandlerDict.ContainsKey(session))
        {
            rpcRspHandlerDict.Remove(session);
        }
    }

    //获取PRC对应的回调
    public static RpcRspHandler GetHandler(long session)
    {
        RpcRspHandler rpcRspHandler;
        rpcRspHandlerDict.TryGetValue(session, out rpcRspHandler);
        RemoveHandler(session);
        return rpcRspHandler;
    }

    //注册处理服务器主动推送协议的回调函数
    public void RegisterMsgHandler(string name, OnMessageHandler callback)
    {
        if (mCallbacks.ContainsKey(name))
        {
            Debug.LogError("repeate register on pid  = " + name);
            return;
        }
        //Type type = GetPacketType(pid);
        //if (type == null)
        //{
        //    Debug.LogError("please RegisterPacketType pid first , pid =" + name);
        //    return;
        //}
        mCallbacks.Add(name, callback);
    }

    public void OnConnectServer(byte[] bytes)
    {
    }

    //反序列化出PBNetMsg
    private PBNetMsg DecodeNetMsg(byte[] buffers, int size)
    {
        MemoryStream netMs = new MemoryStream(buffers, 0, size);
        var ret = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(netMs, null, typeof(PBNetMsg));
        if (ret == null)
        {
            Debug.LogError("cannot Deserialize msg  PBNetMsg");
            return null;
        }
        PBNetMsg msg = ret as PBNetMsg;
        if (msg == null)
        {
            Debug.LogError("cannot Deserialize msg 2 PBNetMsg ");
            return null;
        }
        netMs.Dispose();
        netMs.Close();

        return msg;
    }

    //处理服务器发送过来的协议函数
    public void OnReceiveMsgFromNet(byte[] bytes)
    {
        Pb.PBNetMsg netMsg = DecodeNetMsg(bytes, bytes.Length);
        int cur_session = netMsg.Session;

        //如果是PRC的返回，那就要调用PRC的回调，如果不是PRC的回调，则要处理服务器主动推送过来的协议
        RpcRspHandler rpcRspHandler = NetMsgDispatcher.GetHandler(cur_session);
        if (rpcRspHandler != null)
        {
            rpcRspHandler(netMsg.Payload);
        }
        else
        {
            OnReceiveMsgFromNet1(bytes);
        }
    }

    //处理服务器主动推动过来的协议
    public void OnReceiveMsgFromNet1(byte[] bytes)
    {
        Pb.PBNetMsg netMsg = DecodeNetMsg(bytes, bytes.Length);
        try
        {
            OnMessageHandler callback = mCallbacks[netMsg.Proto];
            if (callback != null)
            {
                callback(netMsg.Payload);
            }
            else
            {
                Debug.LogError("NetHandler.OnProcess callback Error:  not register callback   proto= " + netMsg.Proto);
            }

        }
        catch (Exception e)
        {
            Debug.LogError("NetHandler.OnProcess callback Error: " + e.Message + "  proto= " + netMsg.Proto + " /n " + e.StackTrace);
        }
    }

    //发送一个PRC的请求
    public void SendMessage<T>(string name, ProtoBuf.IExtensible rpcReq, RpcRspHandler rpcRspHandler = null)
    {
        session += 1;
        if (session >= maxSession)
        {
            session = 0;
        }
        if (rpcRspHandler != null)
        {
            AddHandler(session, rpcRspHandler);
        }

        System.IO.MemoryStream netMsg = new System.IO.MemoryStream();
        ProtoBuf.Meta.RuntimeTypeModel.Default.Serialize(netMsg, rpcReq);
        byte[] buffers = netMsg.ToArray();

        SocketSend(name, buffers);
    }

    //把客户端发送的具体协议内容包装成PBNetMsg并序列化成二进制发送到网络中
    private void SocketSend(string name, byte[] data)
    {
        Pb.PBNetMsg netMsg = new Pb.PBNetMsg();
        netMsg.Proto = name;
        netMsg.Session = session;
        netMsg.Payload = data;

        System.IO.MemoryStream netMs = new System.IO.MemoryStream();
        ProtoBuf.Meta.RuntimeTypeModel.Default.Serialize(netMs, netMsg);
        byte[] message = netMs.ToArray();

        MemoryStream ms = null;
        using (ms = new MemoryStream())
        {
            ms.Position = 0;
            BinaryWriter writer = new BinaryWriter(ms);
            UInt32 msglen = Util.SwapUInt32((UInt32)(message.Length));
            writer.Write(msglen);
            writer.Write(message);
            writer.Flush();
            byte[] payload = ms.ToArray();
            NetworkManager.GetInstance().SendBytesWithoutSize(payload);
        }
    }
}
