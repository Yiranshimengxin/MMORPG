using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagMgr : BaseMgr<BagMgr>
{
    private Dictionary<int, Pb.PBItem> mItems = new Dictionary<int, Pb.PBItem>();

    public void InitItem(List<Pb.PBItem> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            mItems.Add(items[i].itemId, items[i]);
        }
    }

    public void SetItemNum(int itemID, int count)
    {
        Pb.PBItem item = mItems[itemID];
        if (item == null)
            return;

        item.Count = count;
        if (item.Count <= 0)
        {
            mItems.Remove(itemID);
        }
    }

    public Pb.PBItem GetItem(int itemID)
    {
        return mItems[itemID];
    }

    public List<Pb.PBItem> GetItems()
    {
        List<Pb.PBItem> pBItems = new List<Pb.PBItem>();
        foreach (var kvp in mItems)
        {
            pBItems.Add(kvp.Value);
        }

        return pBItems;
    }
}
