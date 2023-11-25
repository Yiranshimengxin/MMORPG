using System;
using GameCore.Entitys;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        private void Awake()
        {
        
        }

        private void Update()
        {
            Fighter fighter = EntityManager.MainPlayer.GetRootGameObject().GetComponent<Fighter>();
            if (fighter != null)
            {
                return;
            }
            if (fighter.GetTarget() == null)
            {
                GetComponent<Text>().text = "N/A";
                return;
            }
            Health health = fighter.GetTarget();
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
        }
    }
}