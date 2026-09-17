using EternalTowers.Gameplay.Enemies;
using UnityEngine;

namespace EternalTowers.Gameplay.Towers
{
    public class TowerProjectile : MonoBehaviour
    {
        [SerializeField] private float spriteRotationOffset = 0f;

        private Enemy target;
        private float damage;
        private float speed;
        private float lifeTime;

        public void Initialize(Enemy targetEnemy, float projectileDamage, float projectileSpeed, float projectileLifeTime)
        {
            target = targetEnemy;
            damage = projectileDamage;
            speed = projectileSpeed;
            lifeTime = projectileLifeTime;
        }

        private void Update()
        {
            if (lifeTime > 0f)
            {
                lifeTime -= Time.deltaTime;
                if (lifeTime <= 0f)
                {
                    Destroy(gameObject);
                    return;
                }
            }

            if (target == null || target.State == EnemyState.Dead || target.State == EnemyState.ReachedGoal)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle + spriteRotationOffset, Vector3.forward);

            if (Vector3.Distance(transform.position, target.transform.position) <= 0.15f)
            {
                target.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
