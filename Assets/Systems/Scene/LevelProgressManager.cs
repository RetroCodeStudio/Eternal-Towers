using System.Collections.Generic;
using UnityEngine;

public class LevelProgressManager : MonoBehaviour
{
    [SerializeField] private List<LevelData> levels = new List<LevelData>();

    public bool IsLevelCompleted(int levelNumber)
    {
        LevelData level = levels.Find(l => l.levelNumber == levelNumber);

        return level != null && level.completed;
    }

    public void CompleteLevel(int levelNumber)
    {
        LevelData level = levels.Find(l => l.levelNumber == levelNumber);

        if (level != null)
        {
            level.completed = true;

            Debug.Log("Nivel " + levelNumber + " marcado como completado.");
        }
        else
        {
            Debug.LogWarning("No se encontró el nivel " + levelNumber + ".");
        }
    }

    public void InitializeLevels(List<LevelData> levelData)
    {
        if (levelData == null)
            return;

        levels = levelData;
    }

    public List<LevelData> GetLevels()
    {
        return levels;
    }

    public void SetLevelCompleted(int levelNumber, bool completed)
{
    LevelData level = levels.Find(l => l.levelNumber == levelNumber);

    if (level != null)
    {
        level.completed = completed;
    }
}
}