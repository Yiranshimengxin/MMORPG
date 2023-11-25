using GameCore.Entitys;
using RPG.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.UI
{
    public class ShowHideUI : MonoBehaviour
    {
        [SerializeField] KeyCode toggleKey = KeyCode.Escape;

        public static ShowHideUI mInstance = null;

        public BagUI uiBag = null;
        public GameObject uiDialog = null;


        public void Awake()
        {
            mInstance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            uiBag.gameObject.SetActive(false);
            uiDialog.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                ShowBag(!uiBag.gameObject.active);
            }
        }

        public void OnBagClick()
        {
            ShowBag(true);
        }

        public void ShowBag(bool show)
        {
            if (show)
            {
                uiBag.gameObject.SetActive(true);
            }
            else
            {
                uiBag.gameObject.SetActive(false);
            }
        }

        public void ShowDialog()
        {
            uiBag.gameObject.SetActive(false);
            uiDialog.SetActive(true);
        }

        public void HideDialog()
        {
            uiBag.gameObject.SetActive(false);
            uiDialog.SetActive(false);
        }

        public DialogueUI DialogueUI()
        {
            return uiDialog.GetComponent<DialogueUI>();
        }    
    }
}