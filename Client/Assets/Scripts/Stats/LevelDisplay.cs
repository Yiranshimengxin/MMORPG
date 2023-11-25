using GameCore.Entitys;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        private void Awake()
        {
        }

        private void Update()
        {
            //BaseStats baseStats = EntityManager.MainPlayer.GetRootGameObject().GetComponent<BaseStats>();
            //GetComponent<Text>().text = String.Format("{0:0}", baseStats.GetLevel());
        }
    }
}