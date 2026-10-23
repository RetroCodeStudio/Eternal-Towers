using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelSelectController : MonoBehaviour
{
    [Header("Level Selection")]
    [SerializeField] private LevelUnlockSystem unlockSystem;
    [SerializeField] private Transform levelContainer;
    [SerializeField] private GameObject levelButtonPrefab;

    private readonly List<LevelConfiguration> levels =
        new List<LevelConfiguration>();

    private void Start()
    {
        LoadLevelsFromBuildSettings();
        BuildButtons();
    }

    private void LoadLevelsFromBuildSettings()
    {
        levels.Clear();

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);

            if (!sceneName.StartsWith(
                "level_", System.StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string numberText = sceneName.Substring("level_".Length);

            if (!int.TryParse(numberText, out int levelNumber))
                continue;

            bool unlocked = unlockSystem != null
                ? unlockSystem.IsLevelUnlocked(levelNumber)
                : levelNumber == 1;

            levels.Add(new LevelConfiguration
            {
                levelNumber = levelNumber,
                levelName = "Nivel " + levelNumber,
                sceneName = sceneName,
                isUnlocked = unlocked
            });
        }

        levels.Sort((a, b) => a.levelNumber.CompareTo(b.levelNumber));

        if (levels.Count == 0)
        {
            Debug.LogWarning(
                "No se encontraron escenas level_1, level_2, etc. " +
                "Revisa la lista de escenas de Build Settings.");
        }
    }

    private void BuildButtons()
    {
        if (levelContainer == null || levelButtonPrefab == null)
        {
            Debug.LogError(
                "LevelSelectController: asigna Level Container " +
                "y Level Button Prefab en el Inspector.");
            return;
        }

        foreach (Transform child in levelContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (LevelConfiguration level in levels)
        {
            GameObject buttonObject =
                Instantiate(levelButtonPrefab, levelContainer);

            Button button = buttonObject.GetComponent<Button>();
            TMP_Text tmpLabel = buttonObject.GetComponentInChildren<TMP_Text>();
            Text label = buttonObject.GetComponentInChildren<Text>();

            string buttonText = level.levelName +
                (level.isUnlocked ? "" : " (Bloqueado)");

            if (tmpLabel != null)
            {
                tmpLabel.text = buttonText;
            }
            else if (label != null)
            {
                label.text = buttonText;
            }

            if (button != null)
            {
                button.interactable = level.isUnlocked;

                LevelConfiguration selectedLevel = level;
                button.onClick.AddListener(
                    () => SelectLevel(selectedLevel));
            }
        }
    }

    private void SelectLevel(LevelConfiguration level)
    {
        if (level == null || !level.isUnlocked)
            return;

        if (unlockSystem != null &&
            !unlockSystem.IsLevelUnlocked(level.levelNumber))
        {
            return;
        }

        SceneManager.LoadScene(level.sceneName);
    }
}