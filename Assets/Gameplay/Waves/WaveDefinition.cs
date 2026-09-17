using System;
using System.Collections.Generic;
using EternalTowers.Gameplay.Enemies;
using UnityEngine;

namespace EternalTowers.Gameplay.Waves
{
    [Serializable]
    public class WaveSpawnEntry
    {
        public Enemy enemyPrefab;
        public int count = 1;
        public float delayBetweenSpawns = 0.75f;
    }

    [CreateAssetMenu(fileName = "WaveDefinition", menuName = "Eternal Towers/Wave Definition")]
    public class WaveDefinition : ScriptableObject
    {
        [SerializeField] private List<WaveSpawnEntry> spawns = new List<WaveSpawnEntry>();

        public IReadOnlyList<WaveSpawnEntry> Spawns => spawns;

        public int TotalEnemies
        {
            get
            {
                int total = 0;
                for (int i = 0; i < spawns.Count; i++)
                {
                    if (spawns[i] != null)
                        total += spawns[i].count;
                }

                return total;
            }
        }
    }
}
