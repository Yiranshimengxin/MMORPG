using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore.Entitys.Atts;
using System.IO;
using System.Xml;
using static UnityEngine.EventSystems.EventTrigger;
using RPG.Attributes;

namespace GameCore.Entitys
{
    public class EntityManager : BaseMgr<EntityManager>
    {
        private GameObject mEntityRoot;
        private Dictionary<eEntityType, Transform> mEntitySubRoots = new Dictionary<eEntityType, Transform>();
        private Dictionary<long, Entity> mEntitys = new Dictionary<long, Entity>();
        public static EntityMainPlayer MainPlayer = null;

        public override void Init(GameObject owner)
        {

            EntityFactory.Instance.Init(owner);
            mEntityRoot = new GameObject("ENTITY_ROOT");

            foreach (eEntityType entityType in System.Enum.GetValues(typeof(eEntityType)))
            {
                GameObject gameObject = new GameObject(entityType.ToString());
                gameObject.transform.parent = mEntityRoot.transform;
                mEntitySubRoots.Add(entityType, gameObject.transform);
            }
        }

        //创建实体
        public Entity CreateEntityByAtt(eEntityType entityType, int entityID, AttCharactor entityAtt)
        {
            if (ExistEntity(entityID))
            {
                Debug.LogError("EntityMgr.CreateEntityByAtt error ,entity exist, entityID = " + entityID);
                return null;
            }

            Entity entity = EntityFactory.Instance.CreateEntity(entityType, entityID);
            if (entity == null)
            {
                Debug.LogError("EntityMgr.CreateEntityByAtt error ,entity == null");
                return null;
            }
            entity.AttCharactor = entityAtt;

            if (entityType == eEntityType.PLAYER_MAIN)
            {
                MainPlayer = (EntityMainPlayer)entity;
            }
            AddEntity(entity);

            entity.SetPosition(entityAtt.position);
            entity.SetForward(entityAtt.forward);
            entity.SetLocalScale(entityAtt.scale);

            return entity;
        }
        public bool ExistEntity(long entityID)
        {
            if (mEntitys.ContainsKey(entityID))
            {
                return true;
            }
            return false;
        }

        public bool IsMainPlayer(Entity entity)
        {
            if (MainPlayer == null)
                return false;
            if (entity.AttCharactor.id == MainPlayer.AttCharactor.id)
                return true;
            return false;

        }

        public bool IsMainPlayer(long entityID)
        {
            if (MainPlayer == null)
                return false;
            if (entityID == MainPlayer.AttCharactor.id)
                return true;
            return false;
        }

        public bool IsOtherPlayer(int entityID)
        {
            Entity e = GetEntity(entityID);
            if (e != null)
            {
                return e.GetEntityType() == eEntityType.PLAYER;
            }

            return false;
        }

        public Entity[] GetAllEntitys()
        {
            return mEntitys.Values.ToArray();
        }

        public Entity GetEntity(long entityID)
        {
            if (!mEntitys.ContainsKey(entityID))
            {
                return null;
            }
            return mEntitys[entityID];
        }

        private void AddEntity(Entity entity)
        {
            entity.OnCreate();
            mEntitys.Add(entity.GetID(), entity);
        }
        public void RemoveEntity(Entity entity)
        {
            RemoveEntity(entity.GetID());
        }
        public void RemoveEntity(long id)
        {
            if (!mEntitys.ContainsKey(id))
            {
                Debug.Log("cannot remove enity not find id = " + id);
                return;
            }
            Entity entity = mEntitys[id];
            entity.OnDestory();
            EntityFactory.Instance.DestroyEntity(entity);
            mEntitys.Remove(id);
        }


        public override void Update()
        {
            base.Update();
            //float deltaTime = UnityEngine.Time.deltaTime;
            //foreach (var entity in mEntitys)
            //{
            //    entity.Value.OnUpdate(deltaTime);
            //}
        }
        public override void LateUpdate()
        {
            base.LateUpdate();

            //float deltaTime = UnityEngine.Time.deltaTime;
            //foreach (var entity in mEntitys)
            //{
            //    entity.Value.OnLateUpdate(deltaTime);
            //}
        }

        public override void Exit()
        {
            base.Exit();
            EntityFactory.Instance.Exit();
            mEntitys.Clear();
            GameObject.Destroy(mEntityRoot);
            mEntityRoot = null;
            foreach (var item in mEntitySubRoots)
            {
                GameObject.Destroy(item.Value);
            }
            mEntitySubRoots.Clear();
            mEntitySubRoots = null;
        }
        public Transform GetTransformRoot(eEntityType entityType)
        {
            if (mEntitySubRoots.ContainsKey(entityType))
            {
                return mEntitySubRoots[entityType];
            }
            Debug.LogError("Cann't find root transform for entity ,entitytype = " + entityType.ToString());
            return mEntityRoot.transform;
        }

    }
}