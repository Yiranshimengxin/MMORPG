
using GameCore.Entitys;
using GameCore.Entitys.Atts;
using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes
{
    public class Mana : MonoBehaviour
    {
        float mana;

        private void Awake() {
            mana = GetMaxMana();
        }

        private void Update() {
            if (mana < GetMaxMana())
            {
                mana += GetRegenRate() * Time.deltaTime;
                if (mana > GetMaxMana())
                {
                    mana = GetMaxMana();
                }
            }
        }

        public float GetMana()
        {
            return mana;
        }

        public float GetMaxMana()
        {
            if (EntityManager.MainPlayer == null)
                return 800;
            return EntityManager.MainPlayer.AttCharactor.mp;
        }

        public float GetRegenRate()
        {
            return EntityManager.MainPlayer.AttCharactor.mp;
        }

        public bool UseMana(float manaToUse)
        {
            if (manaToUse > mana)
            {
                return false;
            }
            mana -= manaToUse;
            return true;
        }

        public object CaptureState()
        {
            return mana;
        }

        public void RestoreState(object state)
        {
            mana = (float) state;
        }
    }
}