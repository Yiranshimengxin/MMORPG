using GameCore;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : BaseMgr<GameManager>
{
    public override void Init(GameObject owner)
    {
        NetMsgDispatcher.GetInstance().Init();
    }


    // ���س�����������object
    public void LoadGameScene()
    {
        ResMgr.Instance.LoadCustomScene("Village", () => {

            Pb.PBEnterSceneReq req = new Pb.PBEnterSceneReq();
            NetMsgDispatcher.GetInstance().SendMessage<Pb.PBLoginReq>("PBEnterSceneReq", req, async (buffers) =>
            {
                MemoryStream ms = new MemoryStream(buffers, 0, buffers.Length);
                var ret = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(ms, null, typeof(Pb.PBEnterSceneRsp));
                if (ret == null)
                {
                    Debug.LogError("cannot Deserialize msg , PBLoginRsp ");
                    return;
                }

                Pb.PBEnterSceneRsp rsp = ret as Pb.PBEnterSceneRsp;
                SceneMgr.mInstance.OnEnterScene(rsp);
            });
        });
    }
}
