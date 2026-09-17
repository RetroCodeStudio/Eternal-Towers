using System;
using EternalTowers.Gameplay.Enemies;
using EternalTowers.Gameplay.Waves;
using UnityEngine;

namespace EternalTowers.Gameplay.Core
{
    public class GameplayRuntime : MonoBehaviour
    {
        [SerializeField] private GameEconomy economy;
        [SerializeField] private BaseHealth baseHealth;
        [SerializeField] private WaveManager waveManager;

        public event Action<int> BaseHealthChanged;
        public event Action<int> GoldChanged;
        public event Action<int> WaveCompleted;

        private void Awake()
        {
            if (economy == null)
                economy = GetComponentInChildren<GameEconomy>();

            if (baseHealth == null)
                baseHealth = GetComponentInChildren<BaseHealth>();

            if (waveManager == null)
                waveManager = GetComponentInChildren<WaveManager>();
        }

        private void OnEnable()
        {
            if (waveManager != null)
            {
                waveManager.EnemyDefeated += OnEnemyDefeated;
                waveManager.EnemyReachedBase += OnEnemyReachedBase;
                waveManager.WaveCompleted += OnWaveCompleted;
            }
        }

        private void OnDisable()
        {
            if (waveManager != null)
            {
                waveManager.EnemyDefeated -= OnEnemyDefeated;
                waveManager.EnemyReachedBase -= OnEnemyReachedBase;
                waveManager.WaveCompleted -= OnWaveCompleted;
            }
        }

        private void OnEnemyDefeated(Enemy enemy)
        {
            if (enemy == null)
                return;

            economy?.Earn(enemy.Reward);
            GoldChanged?.Invoke(economy != null ? economy.CurrentGold : 0);
        }

        private void OnEnemyReachedBase(Enemy enemy)
        {
            if (enemy == null)
                return;

            int damageTaken = Mathf.Max(1, enemy.BaseDamage);
            int remainingHealth = baseHealth != null ? baseHealth.TakeDamage(damageTaken) : 0;
            BaseHealthChanged?.Invoke(remainingHealth);
        }

        private void OnWaveCompleted(int waveNumber)
        {
            WaveCompleted?.Invoke(waveNumber);
        }
    }
}
