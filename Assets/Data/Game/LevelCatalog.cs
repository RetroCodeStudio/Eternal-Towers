using System.Collections.Generic;

public class LevelCatalog
{
    public List<LevelConfiguration> levels = new List<LevelConfiguration>();

    public LevelConfiguration GetLevel(int levelNumber)
    {
        return levels.Find(level => level.levelNumber == levelNumber);
    }
}