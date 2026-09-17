using System;

[Serializable]
public class LevelConfiguration
{
    public int levelNumber;
    public string levelName;
    public string sceneName = "Level_01";
    public int totalWaves;
    public int difficulty;
    public bool isUnlocked = true;
    public bool isCompleted;
}