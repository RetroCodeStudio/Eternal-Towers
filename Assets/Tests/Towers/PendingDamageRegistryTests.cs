using NUnit.Framework;
using EternalTowers.Gameplay.Enemies;
using EternalTowers.Gameplay.Towers;
using UnityEngine;

namespace EternalTowers.Tests.Towers
{
    public class PendingDamageRegistryTests
    {
        [Test]
        public void ReservationsAreSharedAndReleased()
        {
            GameObject enemyObject = new GameObject("Enemy");
            Enemy enemy = enemyObject.AddComponent<Enemy>();
            enemy.SetMaxHealth(100f);

            object projectileA = new object();
            object projectileB = new object();

            Assert.IsTrue(PendingDamageRegistry.Register(enemy, projectileA, 60f));
            Assert.IsTrue(PendingDamageRegistry.Register(enemy, projectileB, 30f));
            Assert.AreEqual(90f, PendingDamageRegistry.GetPendingDamage(enemy), 0.001f);
            Assert.AreEqual(10f, PendingDamageRegistry.GetRemainingHealth(enemy), 0.001f);

            PendingDamageRegistry.Release(enemy, projectileA);
            Assert.AreEqual(30f, PendingDamageRegistry.GetPendingDamage(enemy), 0.001f);

            PendingDamageRegistry.Release(enemy, projectileB);
            Assert.AreEqual(0f, PendingDamageRegistry.GetPendingDamage(enemy), 0.001f);

            Object.DestroyImmediate(enemyObject);
        }

        [Test]
        public void SelectorPrefersEnemyThatStillNeedsDamage()
        {
            GameObject coveredObject = new GameObject("CoveredEnemy");
            Enemy coveredEnemy = coveredObject.AddComponent<Enemy>();
            coveredEnemy.SetMaxHealth(100f);

            GameObject openObject = new GameObject("OpenEnemy");
            Enemy openEnemy = openObject.AddComponent<Enemy>();
            openEnemy.SetMaxHealth(100f);
            openObject.transform.position = new Vector3(5f, 0f, 0f);

            object projectile = new object();
            PendingDamageRegistry.Register(coveredEnemy, projectile, 100f);

            Enemy selected = TargetSelector.SelectTarget(
                EnemyRegistry.ActiveEnemies,
                Vector3.zero,
                TargetPriority.Nearest,
                10f,
                20f);

            Assert.AreSame(openEnemy, selected);

            Object.DestroyImmediate(coveredObject);
            Object.DestroyImmediate(openObject);
        }

        [Test]
        public void SelectorFallsBackWhenEveryEnemyIsCovered()
        {
            GameObject enemyObject = new GameObject("CoveredEnemy");
            Enemy enemy = enemyObject.AddComponent<Enemy>();
            enemy.SetMaxHealth(100f);

            object projectile = new object();
            PendingDamageRegistry.Register(enemy, projectile, 100f);

            Enemy selected = TargetSelector.SelectTarget(
                EnemyRegistry.ActiveEnemies,
                Vector3.zero,
                TargetPriority.Nearest,
                10f,
                20f);

            Assert.AreSame(enemy, selected);
            Object.DestroyImmediate(enemyObject);
        }
    }
}
