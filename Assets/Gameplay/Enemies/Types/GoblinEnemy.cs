using System.Collections.Generic;
using UnityEngine;

namespace EternalTowers.Gameplay.Enemies
{
    public class GoblinEnemy : Enemy
    {
        [Header("Damage Aura")]
        [SerializeField, Min(0f)] private float auraRadius = 3f;
        [SerializeField, Range(0f, 100f)] private float damageReductionPercent = 15f;
        [SerializeField, Min(0.05f)] private float auraRefreshInterval = 0.2f;

        private readonly HashSet<Enemy> affectedEnemies = new HashSet<Enemy>();
        private float nextAuraRefreshTime;

        protected override void OnEnable()
        {
            base.OnEnable();
            RefreshAura();
        }

        private void Update()
        {
            if (Time.time < nextAuraRefreshTime)
                return;

            RefreshAura();
        }

        protected override void OnDisable()
        {
            RemoveAura();
            base.OnDisable();
        }

        private void RefreshAura()
        {
            RemoveAura();
            nextAuraRefreshTime = Time.time + auraRefreshInterval;

            Enemy[] enemies = FindObjectsByType<Enemy>();
            float radiusSquared = auraRadius * auraRadius;

            foreach (Enemy enemy in enemies)
            {
                if (enemy == this || enemy.State == EnemyState.Dead || enemy.State == EnemyState.ReachedGoal)
                    continue;

                if ((enemy.transform.position - transform.position).sqrMagnitude > radiusSquared)
                    continue;

                enemy.SetDamageReductionSource(this, damageReductionPercent);
                affectedEnemies.Add(enemy);
            }
        }

        private void RemoveAura()
        {
            foreach (Enemy enemy in affectedEnemies)
            {
                if (enemy != null)
                    enemy.RemoveDamageReductionSource(this);
            }

            affectedEnemies.Clear();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, auraRadius);
        }
    }
}