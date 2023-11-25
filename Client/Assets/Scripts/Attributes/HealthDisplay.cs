using GameCore.Entitys;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    // hud ÑªÁ¿ÏÔÊ¾
    public class HealthDisplay : MonoBehaviour
    {
        private void Awake()
        {
            
        }

        private void Update()
        {
            Health health = EntityManager.MainPlayer.GetRootGameObject().GetComponent<Health>();
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
        }
    }
}