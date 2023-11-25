using RPG.Stats;
using System.Collections;
using UnityEngine;
namespace GameCore.Entitys
{
    public class EntityPlayer : Entity
    {
        public EntityPlayer() : base()
        {
            
        }
        public override void OnCreate()
        {
            base.OnCreate();

            string path = "Assets/Game/Characters/Player/Player.prefab";
            mRootGameObject = ResMgr.Instance.LoadInstantiateObject(path, true) as GameObject;
            mRootTransform = mRootGameObject.transform;

            mRootTransform.parent = EntityManager.Instance.GetTransformRoot(eEntityType.PLAYER);

            mRootGameObject.GetComponent<BaseStats>().objectId = GetID();
        }
        public override eEntityType GetEntityType()
        {
            return eEntityType.PLAYER;
        }

    }
}