using GameCore.Entitys;
using Pb;
using RPG.Attributes;
using RPG.Movement;
using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FighterManager : BaseMgr<FighterManager>
{
    public override void Init(GameObject owner)
    {
        NetMsgDispatcher.GetInstance().RegisterMsgHandler("PBAttackNotify", OnDamage);
        NetMsgDispatcher.GetInstance().RegisterMsgHandler("PBObjectDieNotify", OnDead);
        NetMsgDispatcher.GetInstance().RegisterMsgHandler("PBMoveNotify", OnMove);
        NetMsgDispatcher.GetInstance().RegisterMsgHandler("PBUpdateResourceNotify", OnUpdateResource);
    }


    private void OnDamage(byte[] buffers)
    {
        MemoryStream netMs = new MemoryStream(buffers, 0, buffers.Length);
        var ret = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(netMs, null, typeof(PBAttackNotify));
        PBAttackNotify msg = ret as PBAttackNotify;

        // ��ȡ��������
        Entity attacked = EntityManager.Instance.GetEntity(msg.AttackedObjId);
        if (attacked == null)
        {
            return;
        }

        // ��ȡ������
        Entity attack = EntityManager.Instance.GetEntity(msg.AttackObjId);
        attacked.GetRootGameObject().GetComponent<Health>().TakeDamage(attack.GetRootGameObject(), msg.Damage);
    }

    private void OnDead(byte[] buffers)
    {
        MemoryStream netMs = new MemoryStream(buffers, 0, buffers.Length);
        var ret = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(netMs, null, typeof(PBObjectDieNotify));
        PBObjectDieNotify msg = ret as PBObjectDieNotify;

        Entity deadObj = EntityManager.Instance.GetEntity(msg.objId);
        if (deadObj == null)
        {
            return;
        }
        deadObj.GetRootGameObject().GetComponent<Health>().OnDead();
    }

    private void OnMove(byte[] buffers)
    {
        MemoryStream netMs = new MemoryStream(buffers, 0, buffers.Length);
        var ret = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(netMs, null, typeof(PBMoveNotify));
        PBMoveNotify msg = ret as PBMoveNotify;

        Entity entity = EntityManager.Instance.GetEntity(msg.objId);
        if (entity == null)
        {
            return;
        }

        entity.GetRootGameObject().GetComponent<Mover>().StartMoveAction(new Vector3(msg.positionX, msg.positionY, msg.positionZ), msg.Speed);
    }

    private void OnUpdateResource(byte[] buffers)
    {
        MemoryStream netMs = new MemoryStream(buffers, 0, buffers.Length);
        var ret = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(netMs, null, typeof(PBUpdateResourceNotify));
        PBUpdateResourceNotify msg = ret as PBUpdateResourceNotify;

        GameObject mainObject = EntityManager.MainPlayer.GetRootGameObject();
        if (mainObject == null)
        {
            return;
        }

        Experience experience = mainObject.GetComponent<Experience>();
        if (experience == null)
        {
            return;
        }
        EntityManager.MainPlayer.AttCharactor.level = msg.Level;
        experience.GainExperience(msg.Exp);

        BagMgr.Instance.SetMoney(msg.Money);

        for (int i = 0; i < msg.Items.Count; i++)
        {
            BagMgr.Instance.SetItemNum(msg.Items[i].itemId, msg.Items[i].Count);
        }
    }
}
