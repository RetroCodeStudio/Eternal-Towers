using UnityEngine;
using UnityEngine.EventSystems;
using EternalTowers.Gameplay.UI;

public class TowerSelectionController : MonoBehaviour
{
    [SerializeField] private LayerMask towerLayer;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TowerPlacementSystem placementSystem;
    [SerializeField] private TowerBuildMenu buildMenu;
    [SerializeField] private SelectedTowerPanel selectedTowerPanel;

    private TowerController selectedTower;
    private Vector2 selectedBuildPosition;

    public TowerController SelectedTower => selectedTower;

    public void SetInteractionReferences(
        TowerPlacementSystem placementSystemRef,
        TowerBuildMenu buildMenuRef,
        SelectedTowerPanel selectedTowerPanelRef)
    {
        placementSystem = placementSystemRef;
        buildMenu = buildMenuRef;
        selectedTowerPanel = selectedTowerPanelRef;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearSelection();
            return;
        }

        if (Input.GetMouseButtonDown(0))
            HandlePointerClick();
    }

    private void HandlePointerClick()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            if (buildMenu != null && buildMenu.IsOpen && !buildMenu.ContainsScreenPoint(Input.mousePosition))
                buildMenu.Close();

            return;
        }

        Vector2 worldPosition;
        Camera cameraToUse = mainCamera != null ? mainCamera : Camera.main;
        if (placementSystem != null)
            worldPosition = placementSystem.GetMouseWorldPosition();
        else if (cameraToUse != null)
        {
            Vector3 screenPosition = Input.mousePosition;
            screenPosition.z = Mathf.Abs(cameraToUse.transform.position.z);
            Vector3 worldPoint = cameraToUse.ScreenToWorldPoint(screenPosition);
            worldPosition = new Vector2(worldPoint.x, worldPoint.y);
        }
        else
        {
            ClearSelection();
            return;
        }

        TowerController tower = FindTowerAt(worldPosition);
        if (tower != null)
        {
            SelectTower(tower);
            return;
        }

        if (placementSystem != null && placementSystem.CanPlaceTower(worldPosition))
        {
            selectedBuildPosition = worldPosition;
            selectedTower = null;
            selectedTowerPanel?.ClearSelection();

            Camera worldCamera = mainCamera != null ? mainCamera : Camera.main;
            if (buildMenu != null)
                buildMenu.Open(
                    worldPosition,
                    worldCamera,
                    placementSystem.AvailableTowers,
                    placementSystem.Economy,
                    HandleTowerOptionSelected);
            return;
        }

        ClearSelection();
    }

    private TowerController FindTowerAt(Vector2 worldPosition)
    {
        int layerMask = towerLayer.value == 0 ? Physics2D.AllLayers : towerLayer.value;
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition, layerMask);
        for (int hitIndex = 0; hitIndex < hits.Length; hitIndex++)
        {
            TowerController tower = hits[hitIndex].GetComponentInParent<TowerController>();
            if (tower != null)
                return tower;
        }

        return null;
    }

    private void HandleTowerOptionSelected(TowerData towerData)
    {
        if (placementSystem != null && placementSystem.TryPlaceTower(towerData, selectedBuildPosition))
        {
            buildMenu?.Close();
            selectedTowerPanel?.ClearSelection();
        }
    }

    public void SelectTower(TowerController tower)
    {
        selectedTower = tower;
        buildMenu?.Close();
        selectedTowerPanel?.SetSelectedTower(tower);
    }

    public void ClearSelection()
    {
        selectedTower = null;
        buildMenu?.Close();
        selectedTowerPanel?.ClearSelection();
    }
}
