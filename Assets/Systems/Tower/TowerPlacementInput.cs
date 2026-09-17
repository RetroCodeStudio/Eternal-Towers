using UnityEngine;

public class TowerPlacementInput : MonoBehaviour
{
    [SerializeField] private TowerPlacementSystem placementSystem;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private TowerData towerData;
    [SerializeField] private KeyCode placeKey = KeyCode.Mouse0;

    private void Update()
    {
        if (placementSystem == null || towerPrefab == null)
            return;

        if (!Input.GetKeyDown(placeKey))
            return;

        Vector2 position = placementSystem.GetMouseWorldPosition();
        placementSystem.TryPlaceTower(towerPrefab, towerData, position);
    }
}
