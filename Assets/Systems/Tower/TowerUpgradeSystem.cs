using UnityEngine;
using EternalTowers.Gameplay.Core;

public static class TowerUpgradeSystem
{
    public static bool TryUpgrade(TowerController tower, GameEconomy economy)
    {
        if (tower == null || economy == null)
            return false;

        return tower.TryUpgrade(economy);
    }
}
