using GameCore.Entitys;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class ManaDisplay : MonoBehaviour
    {
        private void Awake()
        {

        }

        private void Update()
        {
            Mana mana = EntityManager.MainPlayer.GetRootGameObject().GetComponent<Mana>();
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", mana.GetMana(), mana.GetMaxMana());
        }
    }
}