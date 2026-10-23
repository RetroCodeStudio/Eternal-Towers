using System;
using System.Collections.Generic;
using EternalTowers.Gameplay.Enemies;
using UnityEngine;

namespace EternalTowers.Gameplay.Waves
{
    [Serializable]
    public class WaveEnemyGroup
    {
        public Enemy enemyPrefab;
        [Min(1)] public int count = 1;
    }

    [Serializable]
    public class WaveSpawnPattern
    {
        public List<int> laneIds = new List<int>();

        public bool IsValid => laneIds != null && laneIds.Count > 0;
    }

    [Serializable]
    public class WaveEnemySet
    {
        [Min(1)] public List<int> laneIds = new List<int> { 1 };
        [Min(0f)] public float delayBeforeSet;
        [Min(0.05f)] public float spawnInterval = 0.6f;
        public List<WaveSpawnPattern> spawnPatterns = new List<WaveSpawnPattern>();
        public List<WaveEnemyGroup> enemyGroups = new List<WaveEnemyGroup>();

        public IReadOnlyList<WaveSpawnPattern> SpawnPatterns => spawnPatterns ?? new List<WaveSpawnPattern>();
    }

    [CreateAssetMenu(fileName = "WaveDefinition", menuName = "Eternal Towers/Wave Definition")]
    public class WaveDefinition : ScriptableObject
    {
        [SerializeField] private List<WaveEnemySet> enemySets = new List<WaveEnemySet>();

        public IReadOnlyList<WaveEnemySet> EnemySets => enemySets;

        public int TotalEnemies
        {
            get
            {
                int total = 0;
                for (int i = 0; i < enemySets.Count; i++)
                {
                    WaveEnemySet set = enemySets[i];
                    if (set == null || set.enemyGroups == null)
                        continue;

                    for (int groupIndex = 0; groupIndex < set.enemyGroups.Count; groupIndex++)
                    {
                        WaveEnemyGroup group = set.enemyGroups[groupIndex];
                        if (group != null)
                            total += group.count;
                    }
                }

                return total;
            }
        }
    }
}
