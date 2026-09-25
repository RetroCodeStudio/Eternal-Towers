using System.Collections.Generic;
using EternalTowers.Gameplay.Enemies;
using UnityEngine;

namespace EternalTowers.Gameplay.Towers
{
    public static class PendingDamageRegistry
    {
        private static readonly Dictionary<Enemy, Dictionary<object, float>> pendingDamageByEnemy =
            new Dictionary<Enemy, Dictionary<object, float>>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetRuntimeState()
        {
            pendingDamageByEnemy.Clear();
        }

        public static float GetPendingDamage(Enemy enemy)
        {
            if (enemy == null || !pendingDamageByEnemy.TryGetValue(enemy, out Dictionary<object, float> reservations))
                return 0f;

            float total = 0f;
            foreach (float damage in reservations.Values)
                total += Mathf.Max(0f, damage);

            return total;
        }

        public static float GetRemainingHealth(Enemy enemy)
        {
            if (enemy == null)
                return 0f;

            return Mathf.Max(0f, enemy.CurrentHealth - GetPendingDamage(enemy));
        }

        public static bool Register(Enemy enemy, object projectile, float damage)
        {
            if (enemy == null || projectile == null || damage <= 0f)
                return false;

            if (!pendingDamageByEnemy.TryGetValue(enemy, out Dictionary<object, float> reservations))
            {
                reservations = new Dictionary<object, float>();
                pendingDamageByEnemy.Add(enemy, reservations);
            }

            reservations[projectile] = damage;
            return true;
        }

        public static void Release(Enemy enemy, object projectile)
        {
            if (enemy == null || projectile == null || !pendingDamageByEnemy.TryGetValue(enemy, out Dictionary<object, float> reservations))
                return;

            reservations.Remove(projectile);
            if (reservations.Count == 0)
                pendingDamageByEnemy.Remove(enemy);
        }

        public static void ReleaseAllForEnemy(Enemy enemy)
        {
            if (enemy != null)
                pendingDamageByEnemy.Remove(enemy);
        }
    }
}
