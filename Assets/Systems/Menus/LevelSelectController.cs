using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private LevelCatalog catalog;
    [SerializeField] private LevelUnlockSystem unlockSystem;
    [SerializeField] private Transform levelContainer;
    [SerializeField] private GameObject levelButtonPrefab;

    private void Start()
    {
        if (catalog == null)
            catalog = new LevelCatalog();

        if (catalog.levels == null)
            catalog.levels = new List<LevelConfiguration>();

        if (catalog.levels.Count == 0)
        {
            catalog.levels.Add(new LevelConfiguration { levelNumber = 1, levelName = "Level 1", sceneName = "Level_01", totalWaves = 3, difficulty = 1, isUnlocked = true });
            catalog.levels.Add(new LevelConfiguration { levelNumber = 2, levelName = "Level 2", sceneName = "Level_02", totalWaves = 4, difficulty = 2, isUnlocked = false });
            catalog.levels.Add(new LevelConfiguration { levelNumber = 3, levelName = "Level 3", sceneName = "Level_03", totalWaves = 5, difficulty = 3, isUnlocked = false });
        }

        if (unlockSystem != null)
        {
            for (int i = 0; i < catalog.levels.Count; i++)
            {
                var level = catalog.levels[i];
                level.isUnlocked = unlockSystem.IsLevelUnlocked(level.levelNumber);
            }
        }

        BuildButtons();
    }

    private void BuildButtons()
    {
        if (levelContainer == null || levelButtonPrefab == null)
            return;

        foreach (Transform child in levelContainer)
            Destroy(child.gameObject);

        for (int i = 0; i < catalog.levels.Count; i++)
        {
            LevelConfiguration level = catalog.levels[i];
            GameObject buttonObject = Instantiate(levelButtonPrefab, levelContainer);
            Button button = buttonObject.GetComponent<Button>();
            Text label = buttonObject.GetComponentInChildren<Text>();

            if (label != null)
                label.text = level.levelName + (level.isUnlocked ? " [Unlocked]" : " [Locked]");

            if (button != null)
            {
                button.interactable = level.isUnlocked;
                button.onClick.AddListener(() => SelectLevel(level));
            }
        }
    }

    private void SelectLevel(LevelConfiguration level)
    {
        if (level == null)
            return;

        if (unlockSystem != null && !unlockSystem.IsLevelUnlocked(level.levelNumber))
            return;

        if (!string.IsNullOrEmpty(level.sceneName))
            SceneManager.LoadScene(level.sceneName);
    }
}
