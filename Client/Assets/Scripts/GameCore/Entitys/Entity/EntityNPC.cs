using System.Collections;
using UnityEngine;
namespace GameCore.Entitys
{
    public class EntityNPC : Entity
    {
        // npc 配置Id
        public int ConfigID
        {
            get;
            set;
        }

        public EntityNPC() : base()
        {
            
        }
        public void OnCreate()
        {
            string path = "Assets/Character/NieR/NieR.prefab";
            
        }
        public override eEntityType GetEntityType()
        {
            return eEntityType.NPC;
        }
    }
}