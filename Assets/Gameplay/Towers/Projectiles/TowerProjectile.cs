using EternalTowers.Gameplay.Enemies;
using UnityEngine;

namespace EternalTowers.Gameplay.Towers
{
    public class TowerProjectile : MonoBehaviour
    {
        private Enemy target;
        private float damage;
        private float speed;

        public void Initialize(Enemy targetEnemy, float projectileDamage, float projectileSpeed)
        {
            target = targetEnemy;
            damage = projectileDamage;
            speed = projectileSpeed;
        }

        private void Update()
        {
            if (target == null || target.State == EnemyState.Dead || target.State == EnemyState.ReachedGoal)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, target.transform.position) <= 0.15f)
            {
                target.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
