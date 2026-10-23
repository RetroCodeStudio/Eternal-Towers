using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using EternalTowers.Gameplay.Enemies;
using EternalTowers.Gameplay.Waves;

namespace EternalTowers.Tests.Enemies
{
    public class EnemyTests
    {
        private sealed class RouteFixture
        {
            public readonly int RouteId;
            public readonly GameObject RouteObject;
            public readonly GameObject GridObject;
            public readonly GameObject EntryObject;
            public readonly GameObject ExitObject;
            public readonly Tilemap Tilemap;
            public readonly Tile Tile;
            public readonly EnemyRouteDefinition Route;

            public RouteFixture(int routeId, int cellCount)
            {
                RouteId = routeId;
                RouteObject = new GameObject("TestRoute");
                Route = RouteObject.AddComponent<EnemyRouteDefinition>();
                GridObject = new GameObject("TestGrid");
                Tilemap = GridObject.AddComponent<Tilemap>();
                GridObject.AddComponent<TilemapRenderer>();
                Tile = ScriptableObject.CreateInstance<Tile>();

                for (int x = 0; x < cellCount; x++)
                {
                    for (int y = -1; y <= 1; y++)
                        Tilemap.SetTile(new Vector3Int(x, y, 0), Tile);
                }

                EntryObject = new GameObject("TestEntry");
                EntryObject.transform.position = Tilemap.GetCellCenterWorld(Vector3Int.zero);
                ExitObject = new GameObject("TestExit");
                ExitObject.transform.position = Tilemap.GetCellCenterWorld(new Vector3Int(cellCount - 1, 0, 0));
                Route.Initialize(routeId, Tilemap, EntryObject.transform, ExitObject.transform);
            }

            public void Dispose()
            {
                EnemyRoutePathfinder.Invalidate(RouteId);
                Object.DestroyImmediate(RouteObject);
                Object.DestroyImmediate(EntryObject);
                Object.DestroyImmediate(ExitObject);
                Object.DestroyImmediate(GridObject);
                Object.DestroyImmediate(Tile);
            }
        }

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
        public void Movement_FollowsOrthogonalTilemapRouteToExitOnce()
        {
            RouteFixture route = new RouteFixture(101, 3);

            GameObject enemyObject = new GameObject("Enemy");
            EnemyMovement movement = enemyObject.AddComponent<EnemyMovement>();
            int reachedGoalCount = 0;
            movement.ReachedGoal += () => reachedGoalCount++;
            enemyObject.transform.position = route.EntryObject.transform.position;
            movement.BeginRoute(route.Route, 100f);

            for (int tick = 0; tick < 32 && movement.IsMoving; tick++)
                movement.Tick(0.1f);

            Assert.That(enemyObject.transform.position, Is.EqualTo(route.ExitObject.transform.position));
            Assert.AreEqual(1, reachedGoalCount);
            Assert.IsFalse(movement.IsMoving);

            Object.DestroyImmediate(enemyObject);
            route.Dispose();
        }

        [Test]
        public void WaveEnemySet_CanDefineSpawnPatternsAcrossLanes()
        {
            WaveSpawnPattern pattern = new WaveSpawnPattern
            {
                laneIds = new System.Collections.Generic.List<int> { 1, 2, 3, 1, 2 }
            };

            WaveEnemySet set = new WaveEnemySet
            {
                laneIds = new System.Collections.Generic.List<int> { 1, 2, 3 },
                spawnPatterns = new System.Collections.Generic.List<WaveSpawnPattern> { pattern }
            };

            Assert.AreEqual(1, set.SpawnPatterns.Count);
            Assert.AreEqual(5, set.SpawnPatterns[0].laneIds.Count);
            Assert.AreEqual(3, set.SpawnPatterns[0].laneIds[2]);

            Object.DestroyImmediate(new GameObject());
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
            RouteFixture route = new RouteFixture(102, 2);

            GameObject enemyObject = new GameObject("Enemy");
            EnemyMovement movement = enemyObject.AddComponent<EnemyMovement>();
            enemyObject.transform.position = route.EntryObject.transform.position;
            movement.BeginRoute(route.Route, 2f);
            movement.ApplySpeedMultiplier(1.5f, 0f);

            Assert.AreEqual(3f, movement.CurrentSpeed);

            Object.DestroyImmediate(enemyObject);
            route.Dispose();
        }
    }
}