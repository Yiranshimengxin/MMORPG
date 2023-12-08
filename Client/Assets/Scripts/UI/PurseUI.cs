using GameCore;
using GameCore.Entitys;
using RPG.Inventories;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.UI
{
    public class PurseUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI balanceField;

        Purse playerPurse = null;

        private void Start()
        {
            //playerPurse = GameObject.FindGameObjectWithTag("Player").GetComponent<Purse>();

            //if (playerPurse != null)
            //{
            //    playerPurse.onChange += RefreshUI;
            //}

            //RefreshUI();

            SceneMgr.mInstance.purseUI = this;
        }

        public void OnPlayerInit()
        {
            playerPurse = EntityManager.MainPlayer.GetRootGameObject().GetComponent<Purse>();

            if (playerPurse != null)
            {
                playerPurse.onChange += RefreshUI;
            }

            RefreshUI();
        }

        private void RefreshUI()
        {
            balanceField.text = $"${playerPurse.GetBalance():N2}";
        }

        private void Update()
        {
            balanceField.text = $"${BagMgr.Instance.GetMoney():N2}";
        }
    }
}