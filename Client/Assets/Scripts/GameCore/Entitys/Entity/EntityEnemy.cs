using GameCore.Entitys;
using GameCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using RPG.Stats;

public class EntityEnemy : Entity
{
    public override void OnCreate()
    {
        base.OnCreate();

        string path = "Assets/Game/Characters/Enemies/Enemy.prefab";
        mRootGameObject = ResMgr.Instance.LoadInstantiateObject(path, true) as GameObject;
        mRootTransform = mRootGameObject.transform;

        mRootTransform.parent = EntityManager.Instance.GetTransformRoot(eEntityType.NPC);

        Vector3 position = new Vector3(1.0f, 0.0f, 4.0f);
        mRootTransform.position = position;

        mRootGameObject.GetComponent<BaseStats>().objectId = GetID();
    }
}
