using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : BaseMgr<DataManager>
{
    private DialogData dialogData = new DialogData();
    private ItemData itemData = new ItemData();

    public override void Init(GameObject owner)
    {
        base.Init(owner);

        LoadAllData();
    }

    public DialogData GetDialogData()
    {
        return dialogData;
    }

    public ItemData GetItemData()
    {
        return itemData;
    }

    public void LoadAllData()
    {
        dialogData.LoadData();
        itemData.LoadData();
    }
}
