using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using EternalTowers.Gameplay.Enemies;
using EternalTowers.Gameplay.Towers;

namespace EternalTowers.Tests.Towers
{
    public class TargetSelectorTests
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
        public void SelectTarget_FirstUsesMostAdvancedEnemy()
        {
            RouteFixture routeA = new RouteFixture(201, 11);
            RouteFixture routeB = new RouteFixture(202, 6);

            GameObject enemyAObject = new GameObject("EnemyA");
            Enemy enemyA = enemyAObject.AddComponent<Enemy>();
            enemyAObject.transform.position = routeA.EntryObject.transform.position;
            enemyA.BeginRouteMovement(routeA.Route);
            EnemyMovement movementA = enemyA.GetComponent<EnemyMovement>();
            movementA.Tick(1f);
            movementA.Tick(1f);
            movementA.Tick(1f);

            GameObject enemyBObject = new GameObject("EnemyB");
            Enemy enemyB = enemyBObject.AddComponent<Enemy>();
            enemyBObject.transform.position = routeB.EntryObject.transform.position;
            enemyB.BeginRouteMovement(routeB.Route);

            Enemy selected = TargetSelector.SelectTarget(new[] { enemyA, enemyB }, Vector3.zero, TargetPriority.First, 100f);

            Assert.AreSame(enemyA, selected);

            Object.DestroyImmediate(enemyAObject);
            Object.DestroyImmediate(enemyBObject);
            routeA.Dispose();
            routeB.Dispose();
        }

        [Test]
        public void SelectTarget_NearestUsesClosestEnemy()
        {
            GameObject towerObject = new GameObject("Tower");
            towerObject.transform.position = Vector3.zero;

            GameObject farEnemyObject = new GameObject("FarEnemy");
            Enemy farEnemy = farEnemyObject.AddComponent<Enemy>();
            farEnemyObject.transform.position = new Vector3(10f, 0f, 0f);

            GameObject nearEnemyObject = new GameObject("NearEnemy");
            Enemy nearEnemy = nearEnemyObject.AddComponent<Enemy>();
            nearEnemyObject.transform.position = new Vector3(2f, 0f, 0f);

            Enemy selected = TargetSelector.SelectTarget(new[] { farEnemy, nearEnemy }, towerObject.transform.position, TargetPriority.Nearest, 100f);

            Assert.AreSame(nearEnemy, selected);

            Object.DestroyImmediate(towerObject);
            Object.DestroyImmediate(farEnemyObject);
            Object.DestroyImmediate(nearEnemyObject);
        }
    }
}
