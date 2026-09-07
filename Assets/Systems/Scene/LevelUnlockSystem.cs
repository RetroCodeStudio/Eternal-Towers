using UnityEngine;

public class LevelUnlockSystem : MonoBehaviour
{
    [SerializeField] private int highestUnlockedLevel = 1;

    public bool IsLevelUnlocked(int levelNumber)
    {
        return levelNumber <= highestUnlockedLevel;
    }

    public void UnlockNextLevel()
    {
        highestUnlockedLevel++;
        Debug.Log("Nivel desbloqueado: " + highestUnlockedLevel);
    }

    public void SetHighestUnlockedLevel(int levelNumber)
{
    if (levelNumber < 1)
        levelNumber = 1;

    highestUnlockedLevel = levelNumber;
}
}