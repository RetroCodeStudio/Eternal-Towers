using NUnit.Framework;
using UnityEngine;
using EternalTowers.Gameplay.Enemies;
using EternalTowers.Gameplay.Towers;

namespace EternalTowers.Tests.Towers
{
    public class TargetSelectorTests
    {
        [Test]
        public void SelectTarget_FirstUsesMostAdvancedEnemy()
        {
            GameObject pathA = new GameObject("PathA");
            EnemyPath enemyPathA = pathA.AddComponent<EnemyPath>();
            GameObject pointA1 = new GameObject("A1");
            GameObject pointA2 = new GameObject("A2");
            pointA1.transform.position = Vector3.zero;
            pointA2.transform.position = new Vector3(10f, 0f, 0f);
            enemyPathA.SetWaypoints(pointA1.transform, pointA2.transform);

            GameObject enemyAObject = new GameObject("EnemyA");
            Enemy enemyA = enemyAObject.AddComponent<Enemy>();
            enemyA.SetPath(enemyPathA);
            enemyA.BeginMovement();
            enemyAObject.transform.position = pointA2.transform.position;

            GameObject pathB = new GameObject("PathB");
            EnemyPath enemyPathB = pathB.AddComponent<EnemyPath>();
            GameObject pointB1 = new GameObject("B1");
            GameObject pointB2 = new GameObject("B2");
            pointB1.transform.position = Vector3.zero;
            pointB2.transform.position = new Vector3(6f, 0f, 0f);
            enemyPathB.SetWaypoints(pointB1.transform, pointB2.transform);

            GameObject enemyBObject = new GameObject("EnemyB");
            Enemy enemyB = enemyBObject.AddComponent<Enemy>();
            enemyB.SetPath(enemyPathB);
            enemyB.BeginMovement();
            enemyBObject.transform.position = pointB1.transform.position;

            Enemy selected = TargetSelector.SelectTarget(new[] { enemyA, enemyB }, Vector3.zero, TargetPriority.First, 100f);

            Assert.AreSame(enemyA, selected);

            Object.DestroyImmediate(pathA);
            Object.DestroyImmediate(pathB);
            Object.DestroyImmediate(pointA1);
            Object.DestroyImmediate(pointA2);
            Object.DestroyImmediate(pointB1);
            Object.DestroyImmediate(pointB2);
            Object.DestroyImmediate(enemyAObject);
            Object.DestroyImmediate(enemyBObject);
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
