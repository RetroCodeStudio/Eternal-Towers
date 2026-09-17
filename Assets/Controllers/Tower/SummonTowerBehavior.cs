using EternalTowers.Gameplay.Enemies;
using UnityEngine;

public class SummonTowerBehavior : TowerAttackBehavior
{
    [SerializeField] private Transform summonPoint;

    private float nextSummonTime;

    public override void Tick()
    {
        if (towerController == null || currentLevel == null)
            return;

        if (Time.time < nextSummonTime)
            return;

        SpawnUnits();
        nextSummonTime = Time.time + Mathf.Max(0.5f, currentLevel.summonCooldown);
    }

    private void SpawnUnits()
    {
        if (currentLevel == null || currentLevel.summonPrefab == null)
            return;

        int count = Mathf.Max(1, currentLevel.summonCount);

        for (int i = 0; i < count; i++)
        {
            Vector3 offset = new Vector3(
                Random.Range(-0.8f, 0.8f),
                Random.Range(-0.8f, 0.8f),
                0f);

            Vector3 spawn = summonPoint != null ? summonPoint.position + offset : transform.position + offset;
            GameObject unitObject = Instantiate(currentLevel.summonPrefab, spawn, Quaternion.identity);

            SummonUnitController ally = unitObject.GetComponent<SummonUnitController>();
            if (ally != null)
                ally.Initialize(transform, currentLevel.damage);
        }
    }
}
