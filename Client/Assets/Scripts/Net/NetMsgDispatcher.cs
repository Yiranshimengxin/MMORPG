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
public class NetMsgDispatcher
{
    public delegate void OnMessageHandler(byte[] rpcRsp);
    private Dictionary<string, OnMessageHandler> mCallbacks = new Dictionary<string, OnMessageHandler>();
    static NetMsgDispatcher _instance = null;
    private static Dictionary<long, RpcRspHandler> rpcRspHandlerDict;
    private int session = 0;
    private int maxSession = int.MaxValue;
    private NetMsgDispatcher(){}
    public static NetMsgDispatcher GetInstance()
    {
        if (_instance != null)
            return _instance;
        _instance = new NetMsgDispatcher();
        return _instance;
    }

    public void Init()
    {
        rpcRspHandlerDict = new Dictionary<long, RpcRspHandler>();
        NetworkManager.GetInstance().OnReceiveMsgCallBack += OnReceiveMsgFromNet;
    }

    private static void AddHandler(long session, RpcRspHandler rpcRspHandler)
    {
        rpcRspHandlerDict.Add(session, rpcRspHandler);
    }

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

    private static void RemoveHandler(long session)
    {
        if (rpcRspHandlerDict.ContainsKey(session))
        {
            rpcRspHandlerDict.Remove(session);
        }
    }

    public static RpcRspHandler GetHandler(long session)
    {
        RpcRspHandler rpcRspHandler;
        rpcRspHandlerDict.TryGetValue(session, out rpcRspHandler);
        RemoveHandler(session);
        return rpcRspHandler;
    }

    public void OnConnectServer(byte[] bytes)
    {
    }

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

    public void OnReceiveMsgFromNet(byte[] bytes)
    {
        Pb.PBNetMsg netMsg = DecodeNetMsg(bytes, bytes.Length);
        int cur_session = netMsg.Session;
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


    public void SendMessage<T>(string name, ProtoBuf.IExtensible rpcReq, RpcRspHandler rpcRspHandler = null)
    {
        session += 1;
        if (session >= maxSession)
            session = 0;
        if (rpcRspHandler != null)
        {
            AddHandler(session, rpcRspHandler);
        }

        System.IO.MemoryStream netMsg = new System.IO.MemoryStream();
        ProtoBuf.Meta.RuntimeTypeModel.Default.Serialize(netMsg, rpcReq);
        byte[] buffers = netMsg.ToArray();

        SocketSend(name, buffers);
    }

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
