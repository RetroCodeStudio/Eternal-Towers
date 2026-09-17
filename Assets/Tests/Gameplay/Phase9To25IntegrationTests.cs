using NUnit.Framework;
using UnityEngine;

namespace EternalTowers.Tests.Gameplay
{
    public class Phase9To25IntegrationTests
    {
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
