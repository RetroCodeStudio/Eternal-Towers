using UnityEngine;

namespace EternalTowers.Gameplay.Enemies
{
    public class WolfEnemy : Enemy
    {
        [Header("Roar")]
        [SerializeField, Range(0f, 1f)] private float roarChance = 0.1f;
        [SerializeField, Min(0f)] private float roarRadius = 3f;
        [SerializeField, Min(0f)] private float speedIncreasePercent = 25f;
        [SerializeField, Min(0f)] private float roarDuration = 3f;

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);

            if (damage > 0f && State != EnemyState.Dead && Random.value < roarChance)
                Roar();
        }

        private void Roar()
        {
            Enemy[] enemies = FindObjectsByType<Enemy>();
            float radiusSquared = roarRadius * roarRadius;
            float speedMultiplier = 1f + speedIncreasePercent / 100f;

            foreach (Enemy enemy in enemies)
            {
                if (enemy == this || enemy.State == EnemyState.Dead || enemy.State == EnemyState.ReachedGoal)
                    continue;

                if ((enemy.transform.position - transform.position).sqrMagnitude > radiusSquared)
                    continue;

                EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
                movement?.ApplySpeedMultiplier(speedMultiplier, roarDuration);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, roarRadius);
        }
    }
}