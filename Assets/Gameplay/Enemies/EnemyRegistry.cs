using System.Collections.Generic;
using UnityEngine;

namespace EternalTowers.Gameplay.Enemies
{
    public static class EnemyRegistry
    {
        private static readonly HashSet<Enemy> activeEnemies = new HashSet<Enemy>();

        public static IEnumerable<Enemy> ActiveEnemies => activeEnemies;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetRuntimeState()
        {
            activeEnemies.Clear();
        }

        public static void Register(Enemy enemy)
        {
            if (enemy != null)
                activeEnemies.Add(enemy);
        }

        public static void Unregister(Enemy enemy)
        {
            if (enemy != null)
                activeEnemies.Remove(enemy);
        }
    }
}
