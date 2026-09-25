using EternalTowers.Gameplay.Enemies;
using EternalTowers.Gameplay.Towers;
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
        private bool damageReservationRegistered;
        private TowerController ownerTower;
        private bool ownerAssigned;

        public void Initialize(Enemy targetEnemy, float projectileDamage, float projectileSpeed, float projectileLifeTime)
        {
            Initialize(targetEnemy, projectileDamage, projectileSpeed, projectileLifeTime, null);
        }

        public void Initialize(
            Enemy targetEnemy,
            float projectileDamage,
            float projectileSpeed,
            float projectileLifeTime,
            TowerController owner)
        {
            ReleaseDamageReservation();

            target = targetEnemy;
            damage = projectileDamage;
            speed = projectileSpeed;
            lifeTime = projectileLifeTime;
            ownerTower = owner;
            ownerAssigned = true;
            damageReservationRegistered = PendingDamageRegistry.Register(target, this, damage);
        }

        private void OnDisable()
        {
            ReleaseDamageReservation();
        }

        private void Update()
        {
            if (lifeTime > 0f)
            {
                lifeTime -= Time.deltaTime;
                if (lifeTime <= 0f)
                {
                    ReleaseDamageReservation();
                    Destroy(gameObject);
                    return;
                }
            }

            if (ownerAssigned && ownerTower == null)
            {
                ReleaseDamageReservation();
                Destroy(gameObject);
                return;
            }

            if (target == null || target.State == EnemyState.Dead || target.State == EnemyState.ReachedGoal)
            {
                ReleaseDamageReservation();
                Destroy(gameObject);
                return;
            }

            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle + spriteRotationOffset, Vector3.forward);

            if (Vector3.Distance(transform.position, target.transform.position) <= 0.15f)
            {
                ReleaseDamageReservation();
                target.TakeDamage(damage);
                Destroy(gameObject);
            }
        }

        private void ReleaseDamageReservation()
        {
            if (!damageReservationRegistered)
                return;

            PendingDamageRegistry.Release(target, this);
            damageReservationRegistered = false;
        }
    }
}
