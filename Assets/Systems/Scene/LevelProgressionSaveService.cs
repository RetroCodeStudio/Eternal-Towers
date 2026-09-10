using UnityEngine;

public class LevelProgressionSaveService
{
    private const string CurrentLevelKey = "EternalTowers_CurrentLevel";
    private const string HighestUnlockedLevelKey = "EternalTowers_HighestUnlockedLevel";

    public void Save(int currentLevel, int highestUnlockedLevel, LevelProgressManager progressManager)
    {
        PlayerPrefs.SetInt(CurrentLevelKey, currentLevel);
        PlayerPrefs.SetInt(HighestUnlockedLevelKey, highestUnlockedLevel);

        for (int levelNumber = 1; levelNumber <= 10; levelNumber++)
        {
            bool completed = progressManager.IsLevelCompleted(levelNumber);

            PlayerPrefs.SetInt(
                "EternalTowers_Level_" + levelNumber + "_Completed",
                completed ? 1 : 0
            );
        }

        PlayerPrefs.Save();

        Debug.Log("Progreso de niveles guardado.");
    }

    public bool HasSavedProgress()
    {
        return PlayerPrefs.HasKey(CurrentLevelKey);
    }

    public int LoadCurrentLevel()
    {
        return PlayerPrefs.GetInt(CurrentLevelKey, 1);
    }

    public int LoadHighestUnlockedLevel()
    {
        return PlayerPrefs.GetInt(HighestUnlockedLevelKey, 1);
    }

    public bool LoadLevelCompleted(int levelNumber)
    {
        return PlayerPrefs.GetInt(
            "EternalTowers_Level_" + levelNumber + "_Completed",
            0
        ) == 1;
    }
}