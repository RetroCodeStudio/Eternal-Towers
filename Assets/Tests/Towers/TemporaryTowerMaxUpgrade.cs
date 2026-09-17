using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class TemporaryTowerMaxUpgrade : MonoBehaviour
{
    [SerializeField] private bool upgradeOnClick = true;
    [SerializeField] private bool logUpgradeSteps = true;

    private TowerController tower;

    public void SetTower(TowerController towerController)
    {
        tower = towerController;
    }

    private void Awake()
    {
        tower = GetComponent<TowerController>();
        if (tower == null)
            tower = GetComponentInParent<TowerController>();
    }

    private void OnMouseDown()
    {
        if (!upgradeOnClick)
            return;

        UpgradeToMaximum();
    }

    public void UpgradeToMaximum()
    {
        if (tower == null)
        {
            Debug.LogWarning("TemporaryTowerMaxUpgrade requiere un TowerController en el mismo objeto o en un padre.", this);
            return;
        }

        int initialLevel = tower.UpgradeLevel;

        while (tower.CanUpgrade)
            tower.ApplyUpgrade();

        if (logUpgradeSteps && tower.UpgradeLevel != initialLevel)
        {
            Debug.Log(
                "Prueba temporal: " + tower.TowerName +
                " subió de nivel " + initialLevel +
                " a " + tower.UpgradeLevel + ".",
                this);
        }
    }
}
