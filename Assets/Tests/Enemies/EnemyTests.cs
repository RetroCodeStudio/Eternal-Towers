using NUnit.Framework;
using UnityEngine;
using EternalTowers.Gameplay.Enemies;

namespace EternalTowers.Tests.Enemies
{
    public class EnemyTests
    {
        [Test]
        public void Enemy_StartsWithConfiguredHealth()
        {
            GameObject enemyObject = new GameObject("Enemy");
            Enemy enemy = enemyObject.AddComponent<Enemy>();

            Assert.AreEqual(enemy.MaxHealth, enemy.CurrentHealth);
            Assert.AreEqual(EnemyState.Spawning, enemy.State);

            Object.DestroyImmediate(enemyObject);
        }

        [Test]
        public void TakeDamage_ReducesHealthAndDiesAtZero()
        {
            GameObject enemyObject = new GameObject("Enemy");
            Enemy enemy = enemyObject.AddComponent<Enemy>();

            enemy.TakeDamage(3f);

            Assert.AreEqual(enemy.MaxHealth - 3f, enemy.CurrentHealth);
            Assert.AreNotEqual(EnemyState.Dead, enemy.State);

            Object.DestroyImmediate(enemyObject);
        }

        [Test]
        public void Enemy_DiesWhenHealthReachesZero()
        {
            GameObject enemyObject = new GameObject("Enemy");
            Enemy enemy = enemyObject.AddComponent<Enemy>();

            enemy.TakeDamage(enemy.MaxHealth);

            Assert.AreEqual(EnemyState.Dead, enemy.State);
            Object.DestroyImmediate(enemyObject);
        }

        [Test]
        public void ReachGoal_RequestsBaseDamageOnlyOnce()
        {
            GameObject enemyObject = new GameObject("Enemy");
            Enemy enemy = enemyObject.AddComponent<Enemy>();
            int baseDamageRequests = 0;
            enemy.BaseDamageRequested += _ => baseDamageRequests++;

            enemy.ReachGoal();
            enemy.ReachGoal();

            Assert.AreEqual(EnemyState.ReachedGoal, enemy.State);
            Assert.AreEqual(1, baseDamageRequests);

            Object.DestroyImmediate(enemyObject);
        }

        [Test]
        public void Movement_ReachesDiagonalGoalOnce()
        {
            GameObject pathObject = new GameObject("Path");
            EnemyPath path = pathObject.AddComponent<EnemyPath>();
            GameObject waypointObject = new GameObject("DiagonalGoal");
            waypointObject.transform.position = new Vector3(2f, 2f, 0f);
            path.SetWaypoints(waypointObject.transform);

            GameObject enemyObject = new GameObject("Enemy");
            EnemyMovement movement = enemyObject.AddComponent<EnemyMovement>();
            int reachedGoalCount = 0;
            movement.ReachedGoal += () => reachedGoalCount++;
            movement.Begin(path, 100f);

            movement.Tick(0.1f);
            movement.Tick(0.1f);

            Assert.That(enemyObject.transform.position, Is.EqualTo(waypointObject.transform.position));
            Assert.AreEqual(1, reachedGoalCount);
            Assert.IsFalse(movement.IsMoving);

            Object.DestroyImmediate(pathObject);
            Object.DestroyImmediate(waypointObject);
            Object.DestroyImmediate(enemyObject);
        }

        [Test]
        public void DamageReductionSource_ReducesIncomingDamage()
        {
            GameObject enemyObject = new GameObject("Enemy");
            Enemy enemy = enemyObject.AddComponent<Enemy>();
            object source = new object();

            enemy.SetDamageReductionSource(source, 25f);
            enemy.TakeDamage(4f);

            Assert.AreEqual(enemy.MaxHealth - 3f, enemy.CurrentHealth);

            Object.DestroyImmediate(enemyObject);
        }

        [Test]
        public void SpeedMultiplier_ChangesMovementSpeed()
        {
            GameObject pathObject = new GameObject("Path");
            EnemyPath path = pathObject.AddComponent<EnemyPath>();
            GameObject waypointObject = new GameObject("Waypoint");
            waypointObject.transform.position = Vector3.right;
            path.SetWaypoints(waypointObject.transform);

            GameObject enemyObject = new GameObject("Enemy");
            EnemyMovement movement = enemyObject.AddComponent<EnemyMovement>();
            movement.Begin(path, 2f);
            movement.ApplySpeedMultiplier(1.5f, 0f);

            Assert.AreEqual(3f, movement.CurrentSpeed);

            Object.DestroyImmediate(pathObject);
            Object.DestroyImmediate(waypointObject);
            Object.DestroyImmediate(enemyObject);
        }
    }
}