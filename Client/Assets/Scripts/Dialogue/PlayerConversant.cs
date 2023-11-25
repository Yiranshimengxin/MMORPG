using GameDevTV.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] string playerName;

        AIConversant currentConversant = null;


        public void StartDialogue(AIConversant newConversant)
        {
            currentConversant = newConversant;

            DialogueManager.Instance.LoadDialog(currentConversant.dialogId);
        }

        public void Quit()
        {
            currentConversant = null;

            DialogueManager.Instance.ExitDialog();
        }

        public bool IsActive()
        {
            return DialogueManager.Instance.mShowDialog != null;
        }

        public void Next()
        {
            DialogueManager.Instance.LoadDialog(DialogueManager.Instance.mShowDialog.NextId);

            ShowHideUI.mInstance.DialogueUI().UpdateUI();
        }

        public bool HasNext()
        {
            DialogDataRow nextData = DataManager.Instance.GetDialogData().GetData(DialogueManager.Instance.mShowDialog.NextId);
            return nextData != null;
        }
    }
}