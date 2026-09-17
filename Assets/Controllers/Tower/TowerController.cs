using System.Collections.Generic;
using System.Linq;
using EternalTowers.Gameplay.Enemies;
using EternalTowers.Gameplay.Towers;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField] private TowerData towerData;
    [SerializeField] private TargetPriority targetPriority = TargetPriority.First;
    [SerializeField] private TowerProjectile projectilePrefab;
    [SerializeField] private Transform projectileOrigin;
    [SerializeField] private float projectileSpeed = 10f;

    private float nextAttackTime;
    private Enemy currentTarget;

    public TowerData TowerData => towerData;
    public TargetPriority TargetPriority => targetPriority;

    public string TowerId => towerData != null ? towerData.TowerId : string.Empty;
    public string TowerName => towerData != null ? towerData.TowerName : string.Empty;
    public TowerType TowerType => towerData != null ? towerData.TowerType : default;

    public int Cost => towerData != null ? towerData.Cost : 0;
    public float Damage => towerData != null ? towerData.Damage : 0f;
    public float Range => towerData != null ? towerData.Range : 0f;
    public float AttackSpeed => towerData != null ? towerData.AttackSpeed : 0f;

    public void Initialize(TowerData data)
    {
        towerData = data;
    }

    private void Update()
    {
        if (towerData == null)
            return;

        if (currentTarget == null || !IsValidTarget(currentTarget))
            currentTarget = GetTarget();

        if (currentTarget == null)
            return;

        if (Vector3.Distance(transform.position, currentTarget.transform.position) > Range)
        {
            currentTarget = null;
            return;
        }

        if (Time.time >= nextAttackTime)
        {
            Attack(currentTarget);
        }
    }

    public void Attack(Enemy target)
    {
        if (target == null || towerData == null)
            return;

        if (projectilePrefab != null)
        {
            Vector3 origin = projectileOrigin != null ? projectileOrigin.position : transform.position;
            TowerProjectile projectile = Instantiate(projectilePrefab, origin, Quaternion.identity);
            projectile.Initialize(target, towerData.Damage, projectileSpeed);
        }
        else
        {
            target.TakeDamage(towerData.Damage);
        }

        nextAttackTime = Time.time + GetAttackCooldown();
    }

    private Enemy GetTarget()
    {
        if (towerData == null)
            return null;

        Enemy[] allEnemies = FindObjectsByType<Enemy>();
        IEnumerable<Enemy> validEnemies = allEnemies.Where(IsValidTarget);

        return TargetSelector.SelectTarget(validEnemies, transform.position, targetPriority, Range);
    }

    private bool IsValidTarget(Enemy enemy)
    {
        if (enemy == null)
            return false;

        if (enemy.State == EnemyState.Dead || enemy.State == EnemyState.ReachedGoal)
            return false;

        return Vector3.Distance(transform.position, enemy.transform.position) <= Range;
    }

    private float GetAttackCooldown()
    {
        return AttackSpeed > 0f ? 1f / AttackSpeed : 0.25f;
    }
}