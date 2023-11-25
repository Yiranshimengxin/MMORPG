using GameCore;
using GameCore.Entitys;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    private GameObject mGo;

    // Start is called before the first frame update
    void Start()
    {
        mGo = gameObject;
        DontDestroyOnLoad(mGo);

        GameManager.Instance.Init(mGo);
        ResMgr.Instance.Init(mGo);
        EntityManager.Instance.Init(mGo);
        DataManager.Instance.Init(mGo);
        FighterManager.Instance.Init(mGo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
