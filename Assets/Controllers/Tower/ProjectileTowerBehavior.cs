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

        Enemy target = TargetSelector.SelectTarget(
            EnemyRegistry.ActiveEnemies,
            transform.position,
            towerController.TargetPriority,
            currentLevel.range,
            towerController.CurrentDamage);

        if (target == null)
            return;

        FireProjectile(target);
        nextAttackTime = Time.time + Mathf.Max(0.15f, 1f / Mathf.Max(0.05f, currentLevel.attackSpeed));
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

        projectile.Initialize(
            target,
            towerController.CurrentDamage,
            currentLevel.projectileSpeed,
            currentLevel.projectileLifeTime,
            towerController);
    }
}
