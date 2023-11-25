using GameCore.Entitys;
using RPG.Attributes;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        private void Awake()
        {

        }

        private void Update()
        {
            Experience experience = EntityManager.MainPlayer.GetRootGameObject().GetComponent<Experience>();
            GetComponent<Text>().text = String.Format("{0:0}", experience.GetPoints());
        }
    }
}