using System;
using System.Collections;
using System.Collections.Generic;
using EternalTowers.Gameplay.Enemies;
using UnityEngine;

namespace EternalTowers.Gameplay.Waves
{
    public class WaveManager : MonoBehaviour
    {
        public enum WaveState
        {
            Waiting,
            Starting,
            Spawning,
            Active,
            Completed,
            Finished
        }

        public event Action<Enemy> EnemySpawned;
        public event Action<Enemy> EnemyDefeated;
        public event Action<Enemy> EnemyReachedBase;
        public event Action<int> WaveStarted;
        public event Action<int> WaveCompleted;

        [SerializeField] private EnemySpawner spawner;
        [SerializeField] private List<WaveDefinition> waves = new List<WaveDefinition>();
        [SerializeField] private float preWaveDelay = 1f;
        [SerializeField] private bool autoStartFirstWave = true;

        private int currentWaveIndex = -1;
        private WaveState state = WaveState.Waiting;
        private int activeEnemies;
        private Coroutine spawnRoutine;

        public int CurrentWaveIndex => currentWaveIndex;
        public WaveState State => state;
        public int WaveCount => waves.Count;
        public int ActiveEnemies => activeEnemies;
        public bool HasWave => currentWaveIndex >= 0 && currentWaveIndex < waves.Count;

        private void Start()
        {
            if (autoStartFirstWave)
                StartNextWave();
        }

        public void StartNextWave()
        {
            if (waves == null || waves.Count == 0)
            {
                state = WaveState.Finished;
                return;
            }

            if (state == WaveState.Spawning || state == WaveState.Active)
                return;

            currentWaveIndex++;

            if (currentWaveIndex >= waves.Count)
            {
                state = WaveState.Finished;
                return;
            }

            WaveStarted?.Invoke(currentWaveIndex + 1);
            state = WaveState.Starting;
            spawnRoutine = StartCoroutine(SpawnWaveRoutine(waves[currentWaveIndex]));
        }

        private IEnumerator SpawnWaveRoutine(WaveDefinition wave)
        {
            if (wave == null)
            {
                state = WaveState.Completed;
                yield break;
            }

            state = WaveState.Spawning;
            yield return new WaitForSeconds(preWaveDelay);

            activeEnemies = 0;

            for (int i = 0; i < wave.Spawns.Count; i++)
            {
                WaveSpawnEntry entry = wave.Spawns[i];
                if (entry == null || entry.enemyPrefab == null)
                    continue;

                for (int j = 0; j < entry.count; j++)
                {
                    if (spawner == null)
                        continue;

                    Enemy enemy = spawner.Spawn(entry.enemyPrefab);
                    if (enemy != null)
                    {
                        activeEnemies++;
                        EnemySpawned?.Invoke(enemy);

                        enemy.Died += () => HandleEnemyDead(enemy);
                        enemy.BaseDamageRequested += damage => HandleEnemyReachedBase(enemy, damage);
                    }

                    if (entry.delayBetweenSpawns > 0f)
                        yield return new WaitForSeconds(entry.delayBetweenSpawns);
                }
            }

            state = WaveState.Active;

            while (activeEnemies > 0)
                yield return null;

            state = WaveState.Completed;

            if (currentWaveIndex >= 0)
                WaveCompleted?.Invoke(currentWaveIndex + 1);
        }

        private void HandleEnemyDead(Enemy enemy)
        {
            activeEnemies = Mathf.Max(0, activeEnemies - 1);
            EnemyDefeated?.Invoke(enemy);
        }

        private void HandleEnemyReachedBase(Enemy enemy, int damage)
        {
            activeEnemies = Mathf.Max(0, activeEnemies - 1);
            EnemyReachedBase?.Invoke(enemy);
        }
    }
}
