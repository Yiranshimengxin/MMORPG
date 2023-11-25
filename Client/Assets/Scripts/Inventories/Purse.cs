using System;
using UnityEngine;

namespace RPG.Inventories {

    //  玩家金钱管理
    public class Purse : MonoBehaviour
    {
        long balance = 0;

        public event Action onChange;

        private void Awake() {

        }

        public float GetBalance()
        {
            return balance;
        }

        public void UpdateBalance(long amount)
        {
            balance += amount;
            if (onChange != null)
            {
                onChange();
            }
        }

        public void RestoreState(long state)
        {
            balance = state;
        }

        //public int AddItems(InventoryItem item, int number)
        //{
        //    if (item is CurrencyItem)
        //    {
        //        UpdateBalance(item.GetPrice() * number);
        //        return number;
        //    }
        //    return 0;
        //}
    }
}