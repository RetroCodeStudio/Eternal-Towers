using EternalTowers.Gameplay.Core;
using EternalTowers.Gameplay.Towers;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField] private TowerData towerData;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TowerAttackBehavior attackBehavior;
    [SerializeField] private TargetPriority targetPriority = TargetPriority.First;

    public int UpgradeLevel { get; private set; } = 0;
    public int MaxUpgradeLevel => towerData != null ? towerData.MaxLevel : 0;
    public bool CanUpgrade => towerData != null && UpgradeLevel < towerData.MaxLevel;
    public string TowerName => towerData != null ? towerData.TowerName : "Tower";
    public TowerData TowerData => towerData;
    public TargetPriority TargetPriority => targetPriority;
    public EternalTowers.Gameplay.Towers.TowerLevelDefinition CurrentLevel => towerData != null ? towerData.GetLevel(UpgradeLevel) : null;
    public int UpgradeCost => GetUpgradeCost();
    public Sprite CurrentVisualSprite => CurrentLevel != null ? CurrentLevel.towerSprite : null;
    public float CurrentDamage => CurrentLevel != null ? CurrentLevel.damage : 0f;
    public float CurrentRange => CurrentLevel != null ? CurrentLevel.range : 0f;
    public float CurrentAttackSpeed => CurrentLevel != null ? CurrentLevel.attackSpeed : 0f;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (attackBehavior == null)
            attackBehavior = GetComponent<TowerAttackBehavior>();

        RefreshVisual();
        if (attackBehavior != null)
            attackBehavior.Initialize(this);
    }

    private void Update()
    {
        if (attackBehavior != null)
            attackBehavior.Tick();
    }

    public void Initialize(TowerData data)
    {
        towerData = data;
        UpgradeLevel = 0;
        RefreshVisual();

        if (attackBehavior != null)
            attackBehavior.Initialize(this);
    }

    public int GetUpgradeCost()
    {
        if (towerData == null)
            return 0;

        var nextLevel = towerData.GetNextLevel(UpgradeLevel);
        return nextLevel != null ? nextLevel.upgradeCost : 0;
    }

    public bool TryUpgrade(GameEconomy economy)
    {
        if (!CanUpgrade || economy == null)
            return false;

        int cost = GetUpgradeCost();
        if (!economy.CanAfford(cost))
            return false;

        if (!economy.Spend(cost))
            return false;

        ApplyUpgrade();

        return true;
    }

    public void ApplyUpgrade()
    {
        if (!CanUpgrade)
            return;

        UpgradeLevel++;
        RefreshVisual();

        if (attackBehavior != null)
            attackBehavior.RefreshLevel(CurrentLevel);
    }

    public void RefreshVisual()
    {
        if (spriteRenderer == null)
            return;

        Sprite sprite = CurrentVisualSprite;
        if (sprite == null)
            return;

        spriteRenderer.sprite = sprite;
        spriteRenderer.enabled = true;
    }
}
