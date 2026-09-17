using EternalTowers.Gameplay.Enemies;
using UnityEngine;

public class SummonUnitController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 0.8f;

    private Transform ownerTower;
    private float damage;
    private float nextAttackTime;
    private Enemy currentTarget;

    public void Initialize(Transform tower, float unitDamage)
    {
        ownerTower = tower;
        damage = unitDamage;
    }

    private void Update()
    {
        if (currentTarget == null || currentTarget.State == EnemyState.Dead || currentTarget.State == EnemyState.ReachedGoal)
            currentTarget = FindNearestEnemy();

        if (currentTarget == null)
        {
            if (ownerTower != null)
            {
                Vector3 direction = (ownerTower.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
            }

            return;
        }

        Vector3 toTarget = currentTarget.transform.position - transform.position;
        float distance = toTarget.magnitude;

        if (distance > attackRange)
        {
            Vector3 direction = toTarget.normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            return;
        }

        if (Time.time >= nextAttackTime)
        {
            currentTarget.TakeDamage(damage);
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private Enemy FindNearestEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>();

        Enemy best = null;
        float bestDistance = float.MaxValue;

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null)
                continue;

            if (enemy.State == EnemyState.Dead || enemy.State == EnemyState.ReachedGoal)
                continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = enemy;
            }
        }

        return best;
    }
}
