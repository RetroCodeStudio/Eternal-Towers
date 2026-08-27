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
        }
    }
}