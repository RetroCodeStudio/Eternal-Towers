using UnityEngine;

public abstract class TowerAttackBehavior : MonoBehaviour
{
    protected TowerController towerController;
    protected EternalTowers.Gameplay.Towers.TowerLevelDefinition currentLevel;

    public virtual void Initialize(TowerController owner)
    {
        towerController = owner;
        RefreshLevel(owner != null ? owner.CurrentLevel : null);
    }

    public virtual void RefreshLevel(EternalTowers.Gameplay.Towers.TowerLevelDefinition level)
    {
        currentLevel = level;
    }

    public abstract void Tick();
}
