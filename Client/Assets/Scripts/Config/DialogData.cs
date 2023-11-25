using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogDataRow
{
    public int Id;
    public string Name;
    public string Context;
    public int NpcId;
    public int NextId;
}

public class DialogData 
{
    private Dictionary<int, DialogDataRow> mData = new Dictionary<int, DialogDataRow>();

    public void LoadData()
    {
        DialogDataRow row1 = new DialogDataRow();
        row1.Id = 10001;
        row1.Name = "农民";
        row1.Context = "欢迎来到这个神奇游戏世界，你可以在这里自由的探索";
        row1.NpcId = 10001;
        row1.NextId = 10002;
        mData.Add(row1.Id, row1);

        DialogDataRow row2 = new DialogDataRow();
        row2.Id = 10002;
        row2.Name = string.Empty;
        row2.Context = "我能在这里做什么？";
        row2.NpcId = 0;
        row2.NextId = 10003;
        mData.Add(row2.Id, row2);

        DialogDataRow row3 = new DialogDataRow();
        row3.Id = 10003;
        row3.Name = "农民";
        row3.Context = "你可以在这里种地，打怪，奔跑";
        row3.NpcId = 10001;
        row3.NextId = 10004;
        mData.Add(row3.Id, row3);

        DialogDataRow row4 = new DialogDataRow();
        row4.Id = 10004;
        row4.Name = string.Empty;
        row4.Context = "告辞";
        row4.NpcId = 0;
        row4.NextId = -1;
        mData.Add(row4.Id, row4);
    }

    public DialogDataRow GetData(int id)
    {
        DialogDataRow retVal;
        if (mData.TryGetValue(id, out retVal))
        {
            return retVal;
        }

        return null;
    }
}
