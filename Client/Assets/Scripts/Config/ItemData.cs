using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataRow 
{
    public int Id;
    public string Name;
    public string Description;
    public string Icon;
    public string Type;
    public int CanBeUsed;
    public int UseType;
    public int UseValue;
}

public class ItemData
{
    private Dictionary<int, ItemDataRow> mData = new Dictionary<int, ItemDataRow>();

    public void LoadData()
    {
        ItemDataRow row1 = new ItemDataRow();
        row1.Id = 1001;
        row1.Name = "经验瓶";
        row1.Description = "使用增加经验值。";
        row1.Icon = "Potion_01(Red)_No BG";
        row1.CanBeUsed = 1;
        row1.UseType = 1;
        row1.UseValue = 10;
        mData.Add(row1.Id, row1);

        ItemDataRow row2 = new ItemDataRow();
        row2.Id = 1002;
        row2.Name = "金币饼";
        row2.Description = "使用后增加金币。";
        row2.Icon = "Coin_Gold_02";
        row2.CanBeUsed = 1;
        row2.UseType = 2;
        row2.UseValue = 10;
        mData.Add(row2.Id, row2);
    }

    public ItemDataRow GetData(int id)
    {
        ItemDataRow retVal;
        if (mData.TryGetValue(id, out retVal))
        {
            return retVal;
        }

        return null;
    }
}