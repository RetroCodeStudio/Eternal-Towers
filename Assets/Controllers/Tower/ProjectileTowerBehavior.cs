using EternalTowers.Gameplay.Enemies;
using EternalTowers.Gameplay.Towers;
using UnityEngine;

public class ProjectileTowerBehavior : TowerAttackBehavior
{
    [SerializeField] private Transform projectileSpawnPoint;

    private float nextAttackTime;

    public override void Tick()
    {
        if (towerController == null || currentLevel == null)
            return;

        if (Time.time < nextAttackTime)
            return;

        Enemy target = FindClosestEnemy();

        if (target == null)
            return;

        FireProjectile(target);
        nextAttackTime = Time.time + Mathf.Max(0.15f, 1f / Mathf.Max(0.05f, currentLevel.attackSpeed));
    }

    private Enemy FindClosestEnemy()
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
            if (distance <= currentLevel.range && distance < bestDistance)
            {
                bestDistance = distance;
                best = enemy;
            }
        }

        return best;
    }

    private void FireProjectile(Enemy target)
    {
        if (currentLevel == null || currentLevel.projectilePrefab == null)
            return;

        Vector3 spawnPosition = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position;
        GameObject projectileObject = Instantiate(currentLevel.projectilePrefab, spawnPosition, Quaternion.identity);
        TowerProjectile projectile = projectileObject.GetComponent<TowerProjectile>();

        if (projectile == null)
            return;

        projectile.Initialize(target, currentLevel.damage, currentLevel.projectileSpeed, currentLevel.projectileLifeTime);
    }
}
