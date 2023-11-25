
using RPG.Inventories;
using RPG.Stats;
using RPG.UI;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
namespace GameCore.Entitys
{
    public class EntityMainPlayer : Entity
    {
        public int Money
        {
            get;
            set;
        }


        public EntityMainPlayer() : base()
        {

        }
        public override void OnCreate()
        {
            base.OnCreate();

            string path = "Assets/Game/Characters/Player/Player.prefab";

            mRootGameObject = ResMgr.Instance.LoadInstantiateObject(path, true) as GameObject;
            mRootTransform = mRootGameObject.transform;

            mRootTransform.parent = EntityManager.Instance.GetTransformRoot(eEntityType.PLAYER_MAIN);

            mRootGameObject.GetComponent<BaseStats>().objectId = GetID();
            mRootGameObject.GetComponent<Purse>().RestoreState(400);
        }
        public override eEntityType GetEntityType()
        {
            return eEntityType.PLAYER_MAIN;
        }
    }
}