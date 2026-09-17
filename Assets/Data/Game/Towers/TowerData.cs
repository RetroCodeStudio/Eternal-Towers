using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Eternal Towers/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("Identificación")]
    [SerializeField] private string towerId;
    [SerializeField] private string towerName;
    [SerializeField] private TowerType towerType;

    [Header("Economía")]
    [SerializeField] private int cost;

    [Header("Visual")]
    [SerializeField] private Sprite towerIcon;
    [SerializeField] private List<EternalTowers.Gameplay.Towers.TowerLevelDefinition> levels = new List<EternalTowers.Gameplay.Towers.TowerLevelDefinition>();

    public string TowerId => towerId;
    public string TowerName => towerName;
    public TowerType TowerType => towerType;
    public int Cost => Mathf.Max(0, cost);
    public Sprite TowerIcon => towerIcon;
    public IReadOnlyList<EternalTowers.Gameplay.Towers.TowerLevelDefinition> Levels => levels;
    public int MaxLevel
    {
        get
        {
            if (levels == null || levels.Count == 0)
                return 0;

            int maximumLevel = 0;
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i] != null)
                    maximumLevel = Mathf.Max(maximumLevel, levels[i].level);
            }

            return maximumLevel;
        }
    }

    public EternalTowers.Gameplay.Towers.TowerLevelDefinition GetLevel(int targetLevel)
    {
        if (levels == null || levels.Count == 0)
            return null;

        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i] != null && levels[i].level == targetLevel)
                return levels[i];
        }

        int index = Mathf.Clamp(targetLevel, 0, levels.Count - 1);
        return levels[index];
    }

    public EternalTowers.Gameplay.Towers.TowerLevelDefinition GetNextLevel(int currentLevel)
    {
        if (levels == null || levels.Count == 0)
            return null;

        return GetLevel(currentLevel + 1);
    }

    public Sprite GetLevelSprite(int level)
    {
        EternalTowers.Gameplay.Towers.TowerLevelDefinition levelData = GetLevel(level);
        return levelData != null ? levelData.towerSprite : towerIcon;
    }

    private void OnValidate()
    {
        if (levels == null)
            return;

        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i] == null)
            {
                Debug.LogWarning($"{name}: Levels[{i}] está vacío. Asigna una definición de nivel.", this);
                continue;
            }

            if (levels[i].level != i)
            {
                Debug.LogWarning(
                    $"{name}: Levels[{i}] tiene nivel {levels[i].level}; se esperaba nivel {i}.",
                    this);
            }
        }
    }
}