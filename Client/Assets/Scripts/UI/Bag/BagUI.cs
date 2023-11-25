using GameCore;
using GameDevTV.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BagUI : MonoBehaviour
{
    public static BagUI mInstance = null;

    public Transform mGridTransform;
    public Text ItemName;
    public Text ItemDescription;
    public Button UseBtn;

    private Dictionary<int, GameObject> mItems = new Dictionary<int, GameObject>();

    private int mSelectedItem = -1;


    public void Awake()
    {
        mInstance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        mSelectedItem = -1;
        List<Pb.PBItem> items = BagMgr.Instance.GetItems();
        for (int i = 0; i < items.Count; i++)
        {
            ItemDataRow data = DataManager.Instance.GetItemData().GetData(items[i].itemId);
            UIItem view = AddItem(items[i].itemId);
            view.SetData(data);
            view.SetCount(items[i].Count);
            view.UpdateUI();
        }
    }

    private void OnDisable()
    {
        mSelectedItem = -1;
        foreach (var kvp in mItems)
        {
            GameObject.Destroy(kvp.Value);
        }

        mItems.Clear();
    }

    public UIItem AddItem(int id)
    {
        if (mItems.ContainsKey(id))
        {
            return null;
        }
        GameObject item = CreateItem();
        mItems.Add(id, item);
        UIItem view = item.GetComponent<UIItem>();
        return view;
    }

    private GameObject CreateItem()
    {
        GameObject obj = UIItem.Instantiate();
        obj.transform.SetParent(mGridTransform, false);
        return obj;
    }

    public void OnClose()
    {
        ShowHideUI.mInstance.ShowBag(false);
    }


    public void OnUseItem()
    {
        if (!mItems.ContainsKey(mSelectedItem))
            return;

        Pb.PBUseItemReq req = new Pb.PBUseItemReq();
        req.itemId = mSelectedItem;
        req.Count = 1;
        NetMsgDispatcher.GetInstance().SendMessage<Pb.PBUseItemReq>("PBUseItemReq", req, async (buffers) =>
        {
            MemoryStream ms = new MemoryStream(buffers, 0, buffers.Length);
            var ret = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(ms, null, typeof(Pb.PBUseItemRsp));
            if (ret == null)
            {
                Debug.LogError("cannot Deserialize msg , PBUseItemRsp ");
                return;
            }

            Pb.PBUseItemRsp rsp = ret as Pb.PBUseItemRsp;
            for (int i = 0;i<rsp.Items.Count; i++)
            {
                BagMgr.Instance.SetItemNum(rsp.Items[i].itemId, rsp.Items[i].Count);

                UIItem view = mItems[rsp.Items[i].itemId].GetComponent<UIItem>();
                view.SetCount(rsp.Items[i].Count);
                view.UpdateUI();
            }
        });
    }

    public void ShowItemInfo(int itemId)
    {
        mSelectedItem = itemId;
        ItemDataRow item = DataManager.Instance.GetItemData().GetData(itemId);
        if (item != null)
        {
            ItemName.text = item.Name;
            ItemDescription.text = item.Description;
        }
    }
}
