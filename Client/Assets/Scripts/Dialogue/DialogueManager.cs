using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameDevTV.UI;


public class DialogueManager : BaseMgr<DialogueManager>
{
    public DialogDataRow mShowDialog = null;
    public void LoadDialog(int dialogId)
    {
        mShowDialog = DataManager.Instance.GetDialogData().GetData(dialogId);

        ShowHideUI.mInstance.ShowDialog();
    }

    public void ExitDialog()
    {
        mShowDialog = null;
        ShowHideUI.mInstance.HideDialog();
    }
}

