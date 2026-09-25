using System.Collections.Generic;
using System.Linq;
using EternalTowers.Gameplay.Enemies;
using UnityEngine;

namespace EternalTowers.Gameplay.Towers
{
    public static class TargetSelector
    {
        public static Enemy SelectTarget(IEnumerable<Enemy> enemies, Vector3 origin, TargetPriority priority, float range)
        {
            return SelectTarget(enemies, origin, priority, range, 0f);
        }

        public static Enemy SelectTarget(
            IEnumerable<Enemy> enemies,
            Vector3 origin,
            TargetPriority priority,
            float range,
            float projectileDamage)
        {
            if (enemies == null)
                return null;

            List<Enemy> validTargets = enemies
                .Where(enemy => enemy != null)
                .Where(enemy => enemy.State != EnemyState.Dead && enemy.State != EnemyState.ReachedGoal)
                .Where(enemy => Vector3.Distance(origin, enemy.transform.position) <= range)
                .ToList();

            if (validTargets.Count == 0)
                return null;

            if (projectileDamage > 0f)
            {
                List<Enemy> targetsNeedingDamage = validTargets
                    .Where(enemy => PendingDamageRegistry.GetRemainingHealth(enemy) > 0f)
                    .ToList();

                if (targetsNeedingDamage.Count > 0)
                    validTargets = targetsNeedingDamage;
            }

            switch (priority)
            {
                case TargetPriority.First:
                    return validTargets
                        .OrderByDescending(enemy => GetProgressValue(enemy))
                        .FirstOrDefault();
                case TargetPriority.Last:
                    return validTargets
                        .OrderBy(GetProgressValue)
                        .FirstOrDefault();
                case TargetPriority.Strongest:
                    return validTargets
                        .OrderByDescending(enemy => enemy.MaxHealth)
                        .FirstOrDefault();
                case TargetPriority.Weakest:
                    return validTargets
                        .OrderBy(enemy => enemy.CurrentHealth)
                        .FirstOrDefault();
                case TargetPriority.Farthest:
                    return validTargets
                        .OrderByDescending(enemy => Vector3.Distance(origin, enemy.transform.position))
                        .FirstOrDefault();
                case TargetPriority.Nearest:
                default:
                    return validTargets
                        .OrderBy(enemy => Vector3.Distance(origin, enemy.transform.position))
                        .FirstOrDefault();
            }
        }

        private static float GetProgressValue(Enemy enemy)
        {
            if (enemy == null || enemy.Path == null)
                return 0f;

            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (movement == null)
                return 0f;

            int waypointIndex = movement.CurrentWaypointIndex;
            Transform currentWaypoint = enemy.Path.GetWaypoint(waypointIndex);
            Transform previousWaypoint = enemy.Path.GetWaypoint(waypointIndex - 1);

            if (currentWaypoint == null)
                return waypointIndex;

            float segmentLength = previousWaypoint != null
                ? Vector3.Distance(previousWaypoint.position, currentWaypoint.position)
                : 1f;

            float distanceToCurrent = Vector3.Distance(enemy.transform.position, currentWaypoint.position);
            float localProgress = segmentLength <= 0f ? 0f : 1f - (distanceToCurrent / segmentLength);

            return waypointIndex + localProgress;
        }
    }
}
