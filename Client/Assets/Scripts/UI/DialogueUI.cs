using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.UI;
using GameCore.Entitys;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        PlayerConversant playerConversant;
        [SerializeField] Text AIText;
        [SerializeField] Button nextButton;
        [SerializeField] Button quitButton;
        [SerializeField] Text conversantName;

        // Start is called before the first frame update
        void Start()
        {
            playerConversant = EntityManager.MainPlayer.GetRootGameObject().GetComponent<PlayerConversant>();
            nextButton.onClick.AddListener(() => playerConversant.Next());
            quitButton.onClick.AddListener(() => playerConversant.Quit());

            UpdateUI();
        }

        public void UpdateUI()
        {
            //playerConversant = EntityManager.MainPlayer.GetComponent<PlayerConversant>();
            gameObject.SetActive(playerConversant.IsActive());
            if (!playerConversant.IsActive())
            {
                return;
            }

            DialogDataRow data = DialogueManager.Instance.mShowDialog;
            if (data.Name == "")
            {
                conversantName.text = EntityManager.MainPlayer.AttCharactor.name;
            }
            else
            {
                conversantName.text = data.Name;
            }

            AIText.text = data.Context;
            nextButton.gameObject.SetActive(playerConversant.HasNext());
        }
    }
}