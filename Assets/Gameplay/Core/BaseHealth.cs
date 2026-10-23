using System;
using UnityEngine;

namespace EternalTowers.Gameplay.Core
{
    public class BaseHealth : MonoBehaviour
    {
        public event Action<int, int> HealthChanged;

        [SerializeField] private int maxHealth = 20;
        [SerializeField] private int currentHealth;

        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public int TakeDamage(int amount)
        {
            if (amount <= 0)
                return currentHealth;

            int previousHealth = currentHealth;
            currentHealth = Mathf.Max(0, currentHealth - amount);
            if (currentHealth != previousHealth)
                HealthChanged?.Invoke(currentHealth, maxHealth);
            return currentHealth;
        }

        public int Heal(int amount)
        {
            if (amount <= 0)
                return currentHealth;

            int previousHealth = currentHealth;
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            if (currentHealth != previousHealth)
                HealthChanged?.Invoke(currentHealth, maxHealth);
            return currentHealth;
        }

        public bool IsDestroyed => currentHealth <= 0;
    }
}
