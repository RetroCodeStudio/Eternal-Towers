using UnityEngine;

public class LevelUnlockSystem : MonoBehaviour
{
    [SerializeField] private int highestUnlockedLevel = 1;

    public int HighestUnlockedLevel => highestUnlockedLevel;

    public bool IsLevelUnlocked(int levelNumber)
    {
        return levelNumber <= highestUnlockedLevel;
    }

    public void UnlockNextLevel()
    {
        highestUnlockedLevel = Mathf.Max(1, highestUnlockedLevel + 1);
        Debug.Log("Nivel desbloqueado: " + highestUnlockedLevel);
    }

    public void SetHighestUnlockedLevel(int levelNumber)
    {
        if (levelNumber < 1)
            levelNumber = 1;

        highestUnlockedLevel = levelNumber;
    }
}