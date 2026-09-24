using EternalTowers.Gameplay.Core;
using UnityEngine;

public class TowerPlacementSystem : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform towerParent;
    [SerializeField] private GameEconomy economy;
    [SerializeField] private bool addTemporaryMaxUpgradeOnPlacement = true;

    [Header("Validación")]
    [SerializeField] private Collider2D placementArea;
    [SerializeField] private LayerMask blockedAreaMask;

    public GameEconomy Economy => economy;

    private void Awake()
    {
        if (economy == null)
        {
            economy = FindAnyObjectByType<GameEconomy>();
        }
    }

    public void SetEconomy(GameEconomy economyRef)
    {
        economy = economyRef;
    }

    public bool CanPlaceTower(Vector2 position)
    {
        if (placementArea == null)
        {
            return false;
        }

        if (!placementArea.OverlapPoint(position))
        {
            return false;
        }

        if (Physics2D.OverlapPoint(position, blockedAreaMask) != null)
        {
            return false;
        }

        return true;
    }

    public Vector2 GetMouseWorldPosition()
    {
        Camera cameraToUse = mainCamera != null ? mainCamera : Camera.main;

        if (cameraToUse == null)
        {
            return Vector2.zero;
        }

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(cameraToUse.transform.position.z);

        Vector3 worldPosition = cameraToUse.ScreenToWorldPoint(mousePosition);

        return new Vector2(worldPosition.x, worldPosition.y);
    }

    public bool TryPlaceTower(
        GameObject towerPrefab,
        TowerData towerData,
        Vector2 position)
    {
        if (towerPrefab == null)
        {
            return false;
        }

        if (!CanPlaceTower(position))
        {
            return false;
        }

        int cost = towerData != null ? towerData.Cost : 0;

        if (economy != null &&
            cost > 0 &&
            !economy.CanAfford(cost))
        {
            return false;
        }

        GameObject towerObject = PlaceTower(towerPrefab, position);

        if (towerObject == null)
        {
            return false;
        }

        if (economy != null && cost > 0)
        {
            economy.Spend(cost);
        }

        TowerController towerController =
            towerObject.GetComponent<TowerController>();

        if (towerController != null && towerData != null)
        {
            towerController.Initialize(towerData);

            if (addTemporaryMaxUpgradeOnPlacement &&
                towerObject.GetComponent<TemporaryTowerMaxUpgrade>() == null)
            {
                TemporaryTowerMaxUpgrade temporaryUpgrade =
                    towerObject.AddComponent<TemporaryTowerMaxUpgrade>();

                temporaryUpgrade.SetTower(towerController);
            }
        }

        // Iniciar la animación de construcción después de colocar la torre.
        Animator towerAnimator =
            towerObject.GetComponent<Animator>();

        if (towerAnimator != null)
        {
            towerAnimator.SetTrigger("Construir");
        }

        return true;
    }

    public GameObject PlaceTower(
        GameObject towerPrefab,
        Vector2 position)
    {
        if (towerPrefab == null)
        {
            return null;
        }

        if (!CanPlaceTower(position))
        {
            return null;
        }

        return Instantiate(
            towerPrefab,
            position,
            Quaternion.identity,
            towerParent);
    }
}