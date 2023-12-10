using GameCore.Entitys;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField] GameObject levelUpParticleEffect = null;

        public event Action onLevelUp;

        int currentLevel = 0;

        Experience experience;

        public int objectId;

        private void Awake()
        {
            experience = GetComponent<Experience>();
        }

        private void Start()
        {

        }

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            int newLevel = EntityManager.MainPlayer.AttCharactor.level;
            if (newLevel > currentLevel)
            {
                currentLevel = newLevel;
                LevelUpEffect();
                //onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        public int GetLevel()
        {
            return currentLevel;
        }
    }
}