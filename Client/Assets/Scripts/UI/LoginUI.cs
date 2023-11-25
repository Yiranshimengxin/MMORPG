using GameCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.CinemachineTriggerAction.ActionSettings;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using System.Drawing;
using System.IO;
using System;
using Pb;

public class LoginUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.GetInstance().OnConnectCallBack += OnConnectServer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLogin()
    {
        //ResMgr.Instance.LoadGameScene();

        NetworkManager.GetInstance().SendConnect("127.0.0.1", 10087);
    }

    private void OnConnectServer(byte[] bytes)
    {
        Pb.PBLoginReq req = new Pb.PBLoginReq();
        req.Account = "test";
        NetMsgDispatcher.GetInstance().SendMessage<Pb.PBLoginReq>("PBLoginReq", req, async (buffers) =>
        {
            MemoryStream ms = new MemoryStream(buffers, 0, buffers.Length);
            var ret = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(ms, null, typeof(Pb.PBLoginRsp));
            if (ret == null)
            {
                Debug.LogError("cannot Deserialize msg , PBLoginRsp ");
                return;
            }

            Pb.PBLoginRsp rsp = ret as Pb.PBLoginRsp;
            BagMgr.Instance.InitItem(rsp.Items);
            //ResMgr.Instance.LoadGameScene();
            GameManager.Instance.LoadGameScene();
        });
    }
}
