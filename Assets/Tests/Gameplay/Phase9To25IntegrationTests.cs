using NUnit.Framework;
using UnityEngine;
using EternalTowers.Gameplay.Core;

namespace EternalTowers.Tests.Gameplay
{
    public class Phase9To25IntegrationTests
    {
        [Test]
        public void GameEconomy_NotifiesHudWhenGoldChanges()
        {
            GameEconomy economy = new GameObject("Economy").AddComponent<GameEconomy>();
            int notificationCount = 0;
            int reportedGold = 0;
            economy.GoldChanged += gold =>
            {
                notificationCount++;
                reportedGold = gold;
            };

            economy.Earn(20);
            economy.Spend(30);

            Assert.AreEqual(2, notificationCount);
            Assert.AreEqual(90, reportedGold);
            Object.DestroyImmediate(economy.gameObject);
        }

        [Test]
        public void BaseHealth_NotifiesHudWhenDamagedAndHealed()
        {
            BaseHealth baseHealth = new GameObject("BaseHealth").AddComponent<BaseHealth>();
            int notificationCount = 0;
            int reportedHealth = 0;
            baseHealth.HealthChanged += (health, _) =>
            {
                notificationCount++;
                reportedHealth = health;
            };

            baseHealth.TakeDamage(7);
            baseHealth.Heal(2);

            Assert.AreEqual(2, notificationCount);
            Assert.AreEqual(15, reportedHealth);
            Object.DestroyImmediate(baseHealth.gameObject);
        }

        [Test]
        public void LevelUnlockSystem_UnlocksNextLevel()
        {
            var unlockSystem = new GameObject("UnlockSystem").AddComponent<LevelUnlockSystem>();
            unlockSystem.SetHighestUnlockedLevel(1);

            unlockSystem.UnlockNextLevel();

            Assert.AreEqual(2, unlockSystem.HighestUnlockedLevel);
            Object.DestroyImmediate(unlockSystem.gameObject);
        }

        [Test]
        public void LevelConfiguration_StoresSceneName()
        {
            var level = new LevelConfiguration
            {
                levelNumber = 1,
                levelName = "Test Level",
                sceneName = "Level_01"
            };

            Assert.AreEqual("Level_01", level.sceneName);
        }
    }
}
