using System;
using GameCore.Entitys;
using RPG.Core;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;
//using UnityEngine.Networking.Types;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour
    {
        [SerializeField] TakeDamageEvent takeDamage;
        public UnityEvent onDie;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
        }

        int healthPoints;

        bool wasDeadLastFrame = false;

        private void Awake() {
            healthPoints = GetInitialHealth();
        }

        private int GetInitialHealth()
        {
            return 800;
        }

        private void Start()
        {
            
        }

        private void OnEnable() {
            //GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        private void OnDisable() {
            //GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }

        public bool IsDead()
        {
            return healthPoints <= 0;
        }

        public void TakeDamage(GameObject instigator, int damage)
        {
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            
            if(IsDead())
            {
                OnDead();
                //AwardExperience(instigator);
            } 
            else
            {
                takeDamage.Invoke(damage);
            }
            UpdateState();
        }

        public void OnDead()
        {
            healthPoints = 0;
            onDie.Invoke();
            UpdateState();
        }

        public void Heal(int healthToRestore)
        {
            healthPoints = Mathf.Min(healthPoints + healthToRestore, GetMaxHealthPoints());
            UpdateState();
        }

        public int GetHealthPoints()
        {
            return healthPoints;
        }

        public int GetMaxHealthPoints()
        {
            int objectId = GetComponent<BaseStats>().objectId;
            Entity entity = EntityManager.Instance.GetEntity(objectId);
            return entity.AttCharactor.maxHp;
        }

        public float GetPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            if (GetMaxHealthPoints() <= 0)
                return 1;

            return (float)healthPoints / (float)GetMaxHealthPoints();
        }

        private void UpdateState()
        {
            Animator animator = GetComponent<Animator>();
            if (!wasDeadLastFrame && IsDead())
            {
                animator.SetTrigger("die");
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }

            if (wasDeadLastFrame && !IsDead())
            {
                animator.Rebind();
            }

            wasDeadLastFrame = IsDead();
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(10);
        }

        public void RestoreHp(int hp)
        {
            healthPoints = hp;
            
            UpdateState();
        }
    }
}