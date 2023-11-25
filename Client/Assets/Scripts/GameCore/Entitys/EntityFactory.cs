using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;
namespace GameCore.Entitys
{
    public class EntityFactory : BaseMgr<EntityFactory>
    {
        public Dictionary<eEntityType, List<Entity>> mEntityPools = new Dictionary<eEntityType, List<Entity>>();
        private int mUUID = 0;

        public override void Init(GameObject owner)
        {
        }
        public override void Exit()
        {
            mEntityPools.Clear();
        }


        public Entity CreateEntity(eEntityType entityType,int entityID)
        {
            Entity entity = null;
            if (mEntityPools.ContainsKey(entityType))
            {
                if (mEntityPools[entityType].Count > 0)
                {
                    entity = mEntityPools[entityType][0];
                    mEntityPools[entityType].RemoveAt(0);
                    //entity.AttCharactor.id = entityID;
                    return entity;
                }
            }

            switch (entityType)
            {
                case eEntityType.PLAYER_MAIN:
                    entity = new EntityMainPlayer();
                    break;
                case eEntityType.PLAYER:
                    entity = new EntityPlayer();
                    break;
                case eEntityType.NPC:
                    entity = new EntityNPC();
                    break;
                case eEntityType.ENEMY:
                    entity = new EntityEnemy();
                    break;
                default:
                    Debug.LogError("Faild to create entity : cls " + entityType.ToString());
                    break;
            }
            //entity.AttCharactor.id = entityID;
            return entity;

        }
        public void DestroyEntity(Entity entity)
        {
            eEntityType entityType = entity.GetEntityType();
            List<Entity> pool;
            if (!mEntityPools.TryGetValue(entityType, out pool))
            {
                pool = new List<Entity>();
                mEntityPools[entityType] = pool;
            }
            pool.Add(entity);
        }


        public void Release()
        {
            mEntityPools.Clear();
        }

    }
}