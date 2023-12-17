using GameCore.Entitys;
using GameDevTV.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudUI : MonoBehaviour
{
    public Slider Hp;
    public Text HpText;

    public Slider Mp;
    public Text MpText;

    public Text Leve;

    void Start()
    {
        Hp.value = 0.5f;
        HpText.text = "200/1000";

        Mp.value = 1.0f;
        MpText.text = "100/800";

        Leve.text = "30";
    }

    // Update is called once per frame
    void Update()
    {
        if (EntityManager.MainPlayer == null)
        {
            return;
        }

        Hp.value = (float)EntityManager.MainPlayer.AttCharactor.hp/(float)EntityManager.MainPlayer.AttCharactor.maxHp;
        HpText.text = string.Format("{0}/{1}", EntityManager.MainPlayer.AttCharactor.hp, EntityManager.MainPlayer.AttCharactor.maxHp);

        Mp.value = (float)EntityManager.MainPlayer.AttCharactor.mp/ (float)EntityManager.MainPlayer.AttCharactor.maxMp;
        MpText.text = string.Format("{0}/{1}", EntityManager.MainPlayer.AttCharactor.mp, EntityManager.MainPlayer.AttCharactor.maxMp);

        Leve.text = string.Format("{0}", EntityManager.MainPlayer.AttCharactor.level);
    }

    public void OnBagClick()
    {
        ShowHideUI.mInstance.ShowBag(true);
    }
}
