using UnityEngine;

namespace EternalTowers.Gameplay.Core
{
    public class BaseHealth : MonoBehaviour
    {
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

            currentHealth = Mathf.Max(0, currentHealth - amount);
            return currentHealth;
        }

        public int Heal(int amount)
        {
            if (amount <= 0)
                return currentHealth;

            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            return currentHealth;
        }

        public bool IsDestroyed => currentHealth <= 0;
    }
}
