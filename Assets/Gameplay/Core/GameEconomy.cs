using System;
using UnityEngine;

namespace EternalTowers.Gameplay.Core
{
    public class GameEconomy : MonoBehaviour
    {
        public event Action<int> GoldChanged;

        [SerializeField] private int startingGold = 100;
        [SerializeField] private int currentGold;

        public int CurrentGold => currentGold;

        private void Awake()
        {
            currentGold = startingGold;
        }

        public bool CanAfford(int amount)
        {
            return amount >= 0 && currentGold >= amount;
        }

        public bool Spend(int amount)
        {
            if (!CanAfford(amount))
                return false;

            currentGold -= amount;
            GoldChanged?.Invoke(currentGold);
            return true;
        }

        public void Earn(int amount)
        {
            if (amount <= 0)
                return;

            currentGold += amount;
            GoldChanged?.Invoke(currentGold);
        }
    }
}
