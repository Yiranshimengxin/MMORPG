using GameCore.Entitys;
using GameCore.Entitys.Atts;
using Pb;
using RPG.Attributes;
using RPG.UI;
using UnityEngine;

namespace GameCore
{
    public class SceneMgr : MonoBehaviour
    {
        private GameObject mCameraObj;
        public static SceneMgr mInstance = null;

        public Cinemachine.CinemachineVirtualCamera mFollowCamera;

        public PurseUI purseUI = null;

        public void Awake()
        {
            mInstance = this;
        }

        public void Start()
        {

        }

        public bool Init()
        {
            Debug.Log("SceneMgr.Init :  ");

            return true;
        }

        //override public void Update()
        //{
        //}

        // ��������Լ�
        public void LoadMainPlayer(Pb.SceneObject sceneObject)
        {
            AttCharactor attr = new AttCharactor();
            attr.id = sceneObject.objId;
            attr.name = "test";
            attr.position = new Vector3(sceneObject.positionX, sceneObject.positionY, sceneObject.positionZ); ;
            attr.forward = Quaternion.Euler(0, 0, 0) * Vector3.forward;
            attr.scale = UnityEngine.Vector3.one;

            attr.hp = sceneObject.Hp;
            attr.maxHp = sceneObject.maxHp;
            attr.mp = sceneObject.Mp;
            attr.maxMp = sceneObject.maxMp;
            attr.level = sceneObject.Level;


            EntityMainPlayer mainPlayer = (EntityMainPlayer)EntityManager.Instance.CreateEntityByAtt(eEntityType.PLAYER_MAIN, attr.id, attr);
            mFollowCamera.Follow = mainPlayer.GetRootTransform();
            mainPlayer.GetRootGameObject().GetComponent<Health>().RestoreHp(sceneObject.Hp);
            purseUI.OnPlayerInit();
        }

        public void OnEnterScene(Pb.PBEnterSceneRsp rsp)
        {
            LoadMainPlayer(rsp.Self);

            for (int i = 0; i < rsp.sceneObjects.Count; i++)
            {
                Pb.SceneObject sceneObject = rsp.sceneObjects[i];
                if (sceneObject.Type == (int)eEntityType.PLAYER)
                {
                    LoadPlayer(sceneObject);
                }
                else if (sceneObject.Type == (int)eEntityType.NPC)
                {

                }
                else if (sceneObject.Type == (int)eEntityType.ENEMY)
                {
                    LoadEnemies(sceneObject);
                }
            }
        }

        // �����������
        public void LoadPlayer(Pb.SceneObject objectPlayer)
        {
            AttCharactor attr = new AttCharactor();
            attr.id = objectPlayer.objId;
            attr.name = "other";
            attr.position = new Vector3(objectPlayer.positionX, objectPlayer.positionY, objectPlayer.positionZ);
            attr.forward = Quaternion.Euler(0, 0, 0) * Vector3.forward;
            attr.scale = UnityEngine.Vector3.one;

            attr.hp = objectPlayer.Hp;
            attr.maxHp = objectPlayer.maxHp;


            EntityPlayer player = (EntityPlayer)EntityManager.Instance.CreateEntityByAtt(eEntityType.PLAYER, attr.id, attr);
            player.GetRootGameObject().GetComponent<Health>().RestoreHp(objectPlayer.Hp);
        }

        // ��������
        public void LoadEnemies(Pb.SceneObject objectEnemy)
        {
            AttCharactor attr = new AttCharactor();
            attr.id = objectEnemy.objId;
            attr.name = "����";
            attr.position = new Vector3(objectEnemy.positionX, objectEnemy.positionY, objectEnemy.positionZ);
            attr.forward = Quaternion.Euler(0, 0, 0) * Vector3.forward;
            attr.scale = UnityEngine.Vector3.one;
            attr.hp = objectEnemy.Hp;
            attr.maxHp = objectEnemy.maxHp;

            EntityEnemy enemy = (EntityEnemy)EntityManager.Instance.CreateEntityByAtt(eEntityType.ENEMY, attr.id, attr);
            enemy.GetRootGameObject().GetComponent<Health>().RestoreHp(objectEnemy.Hp);
        }
    }
}