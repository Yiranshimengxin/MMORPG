using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using NetEventType = System.Collections.Generic.KeyValuePair<System.Action<byte[]>, byte[]>;

public enum DisType
{
    Exception,
    Disconnect,
}

//网络管理类
public class NetworkManager : MonoBehaviour
{
    //定义一个单例实例
    static NetworkManager _instance;
    //定义一个只读的锁
    static readonly object m_lockObject = new object();
    //定义一个事件的队列
    static Queue<NetEventType> mEvents = new Queue<NetEventType>();
    //tcp客户端
    private TcpClient client = null;
    //数据流
    private NetworkStream outStream = null;
    private MemoryStream memStream;
    private BinaryReader reader;

    //定义每次从socket中读取的最大数据
    private const int MAX_READ = 8192;
    // private int session = 0;
    // private int maxSession = int.MaxValue/2;
    private byte[] byteBuffer = new byte[MAX_READ];
    // public static bool loggedIn = false;

    //网络事件回调函数
    Action<byte[]> onConnectCallBack = null;
    Action<byte[]> onDisConnectCallBack = null;
    Action<byte[]> onReceiveMsgCallBack = null;
    public Action<byte[]> OnConnectCallBack
    {
        get
        {
            return onConnectCallBack;
        }
        set
        {
            onConnectCallBack = value;
        }
    }
    public Action<byte[]> OnDisConnectCallBack
    {
        get
        {
            return onDisConnectCallBack;
        }
        set
        {
            onDisConnectCallBack = value;
        }
    }
    public Action<byte[]> OnReceiveMsgCallBack
    {
        get
        {
            return onReceiveMsgCallBack;
        }
        set
        {
            onReceiveMsgCallBack = value;
        }
    }

    //获取当前实例
    public static NetworkManager GetInstance()
    {
        return _instance;
    }

    void Awake()
    {
        _instance = this;
        Init();
    }

    //初始化
    void Init()
    {
        memStream = new MemoryStream();
        reader = new BinaryReader(memStream);
    }

    //给事件队列添加队列
    public static void AddEvent(Action<byte[]> _event, byte[] data)
    {
        lock (m_lockObject)
        {
            mEvents.Enqueue(new NetEventType(_event, data));
        }
    }

    //循环函数，如果事件队列中有事件，则取出第一个事件抛出
    void Update()
    {
        if (mEvents.Count > 0)
        {
            while (mEvents.Count > 0)
            {
                NetEventType _event = mEvents.Dequeue();
                _event.Key(_event.Value);
            }
        }
    }

    //建立客户端链接，链接到服务器，支持域名的链接
    public void SendConnect(string host, int port)
    {
        Debug.Log("host : " + host + " port:" + port.ToString());
        this.Close();
        try
        {
            IPAddress[] address = Dns.GetHostAddresses(host);
            if (address.Length == 0)
            {
                Debug.LogError("host invalid");
                return;
            }
            if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
            {
                client = new TcpClient(AddressFamily.InterNetworkV6);
            }
            else
            {
                client = new TcpClient(AddressFamily.InterNetwork);
            }
            client.SendTimeout = 1000;
            client.ReceiveTimeout = 1000;
            //不延迟发送
            client.NoDelay = true;
            Debug.Log("begin connect socket");

            //使用无阻塞的链接方式，链接成功或者失败以回调的方式告诉程序
            client.BeginConnect(host, port, new AsyncCallback(OnConnect), null);
        }
        catch (Exception e)
        {
            Debug.Log("begin connect socket error");
            this.Close();
            Debug.LogError(e.Message);
        }
    }

    //链接的回调，是在于主线程不同的线程当中处理的
    void OnConnect(IAsyncResult asr)
    {
        Debug.Log("on connect : " + client.Connected.ToString());
        //链接失败处理
        if (!client.Connected)
        {
            Close();
            AddEvent(onDisConnectCallBack, null);
            return;
        }
        //链接成功处理
        outStream = client.GetStream();
        {
            client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
        }
        Debug.Log("onConnectCallBack : " + (onConnectCallBack != null).ToString());
        AddEvent(onConnectCallBack, null);
    }

    //发送数据到socket中
    public void SendBytes(byte[] message)
    {
        MemoryStream ms = null;
        using (ms = new MemoryStream())
        {
            ms.Position = 0;
            BinaryWriter writer = new BinaryWriter(ms);
            {
                UInt32 msglen = Util.SwapUInt32((UInt32)(message.Length));
                writer.Write(msglen);
            }
            writer.Write(message);
            writer.Flush();
            if (client != null && client.Connected)
            {
                byte[] payload = ms.ToArray();

                //使用了无阻塞的方式，成功或者失败也是通过回调的方式通知程序
                outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
            }
            else
            {
                Debug.LogError("client.connected----->>false");
            }
        }
    }

    //发送的数据不写入4字节的长度到协议头部
    public void SendBytesWithoutSize(byte[] message)
    {
        if (client != null && client.Connected)
        {
            outStream.BeginWrite(message, 0, message.Length, new AsyncCallback(OnWrite), null);
        }
        else
            Debug.LogError("SendBytesWithoutSize failed:connected----->>false");
    }

    //读取socket中数据的回调，如果socket中有数据，会执行这个函数，如果没有就不执行
    void OnRead(IAsyncResult asr)
    {
        int bytesRead = 0;
        try
        {
            lock (client.GetStream())
            {
                //读取字节流到缓冲区
                bytesRead = client.GetStream().EndRead(asr);
            }
            if (bytesRead < 1)
            {
                //包尺寸有问题，断线处理
                OnDisconnected(DisType.Disconnect, "bytesRead < 1");
                return;
            }
            OnReceive(byteBuffer, bytesRead);   //分析数据包内容，抛给逻辑层
            lock (client.GetStream())
            {
                //分析完，再次监听服务器发过来的新消息
                Array.Clear(byteBuffer, 0, byteBuffer.Length);   //清空数组
                client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
            }
        }
        catch (Exception ex)
        {
            OnDisconnected(DisType.Exception, ex.Message);
        }
    }

    //网络断开的处理，加入到事件队列中，抛给逻辑层处理
    void OnDisconnected(DisType dis, string msg)
    {
        Close();   //关掉客户端链接
        Debug.Log("networkmanager on disconnect!" + msg + " trace:" + new System.Diagnostics.StackTrace().ToString());
        AddEvent(onDisConnectCallBack, null);
    }

    //调试用的函数，打印接收到的数据的内容
    void PrintBytes()
    {
        string returnStr = string.Empty;
        for (int i = 0; i < byteBuffer.Length; i++)
        {
            returnStr += byteBuffer[i].ToString("X2");
        }
        Debug.LogError(returnStr);
    }

    //写入回调，成功或失败都会调用到这个函数
    void OnWrite(IAsyncResult r)
    {
        try
        {
            outStream.EndWrite(r);
        }
        catch (Exception ex)
        {
            Debug.LogError("OnWrite--->>>" + ex.Message);
        }
    }

    //解析接收到的数据，把数据抛给逻辑层处理
    void OnReceive(byte[] bytes, int length)
    {
        memStream.Seek(0, SeekOrigin.End);
        memStream.Write(bytes, 0, length);
        memStream.Seek(0, SeekOrigin.Begin);
        //Debug.Log("on receive RemainingBytes len : " + RemainingBytes().ToString()+ "  length len:" + length.ToString());
        while (RemainingBytes() > 4)
        {
            UInt32 messageLen = reader.ReadUInt32();
            messageLen = Util.SwapUInt32((UInt32)(messageLen));
            if (RemainingBytes() >= messageLen)
            {
                OnReceivedMessage(reader.ReadBytes((int)messageLen));
            }
            else
            {
                memStream.Position = memStream.Position - 4;
                break;
            }
        }
        //Create a new stream with any leftover bytes
        byte[] leftover = reader.ReadBytes((int)RemainingBytes());
        memStream.SetLength(0);     //Clear
        memStream.Write(leftover, 0, leftover.Length);
    }

    long RemainingBytes()
    {
        return memStream.Length - memStream.Position;
    }

    //把解析到的二进制数据添加到事件队列中，抛给逻辑层处理
    void OnReceivedMessage(byte[] cmd_byte)
    {
        AddEvent(onReceiveMsgCallBack, cmd_byte);
    }

    //关闭函数
    void Close()
    {
        if (client != null)
        {
            if (client.Connected)
            {
                client.Close();
            }
            client = null;
        }
        // loggedIn = false;
    }

    //销毁函数，清理资源，释放内存
    void OnDestroy()
    {
        onConnectCallBack = null;
        onDisConnectCallBack = null;
        onReceiveMsgCallBack = null;
        if (client != null)
        {
            if (client.Connected) client.Close();
            client = null;
        }
        // loggedIn = false;
        reader.Close();
        memStream.Close();
        Debug.Log("~NetworkManager was destroy");
    }
}
