using GameCore.Entitys;
using Pb;
using RPG.Attributes;
using RPG.Movement;
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
    }


    private void OnDamage(byte[] buffers)
    {
        MemoryStream netMs = new MemoryStream(buffers, 0, buffers.Length);
        var ret = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(netMs, null, typeof(PBAttackNotify));
        PBAttackNotify msg = ret as PBAttackNotify;

        // 获取被攻击者
        Entity attacked = EntityManager.Instance.GetEntity(msg.AttackedObjId);
        if (attacked == null)
        {
            return;
        }

        // 获取攻击者
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
}
