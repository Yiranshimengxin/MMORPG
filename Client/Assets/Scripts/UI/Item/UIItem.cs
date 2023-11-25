using GameCore;
using GameDevTV.UI;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Purchasing;
using UnityEngine.UI;


public class UIItem : MonoBehaviour, IPointerClickHandler
{
    private const string ItemPath = "Assets/Game/UI/Inventory/InventoryItem.prefab";


    public Image Image;
    public Text Name;
    public Text Num;

    private ItemDataRow mRowData;
    private int mCount;

    public static GameObject Instantiate()
    {
        GameObject obj = ResMgr.Instance.LoadInstantiateObject(ItemPath, true) as GameObject;
        return obj;
    }

    public void SetData(ItemDataRow data)
    {
        mRowData = data;
    }

    public void SetCount(int count)
    {
        mCount = count;
    }

    public void UpdateUI()
    {
        Name.text = string.Format("{0}", mRowData.Name);
        Num.text = string.Format("{0}", mCount);

        string icon = string.Format("Assets/Asset Packs/Universal Gold Icons Sample/{0}.png", mRowData.Icon);
        Sprite iconObject = ResMgr.Instance.LoadResource<Sprite>(icon);
        Image.overrideSprite = iconObject;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("On Item Click");
        ShowHideUI.mInstance.uiBag.ShowItemInfo(mRowData.Id);
    }
}
