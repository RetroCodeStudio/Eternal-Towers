using UnityEngine;
using EternalTowers.Gameplay.UI;

public class TowerSelectionController : MonoBehaviour
{
    [SerializeField] private LayerMask towerLayer;
    [SerializeField] private Camera mainCamera;

    private GameplayHUD hud;
    private TowerController selectedTower;

    public TowerController SelectedTower => selectedTower;

    public void SetHud(GameplayHUD hudRef)
    {
        hud = hudRef;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TrySelectTowerFromMouse();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearSelection();
        }
    }

    public void TrySelectTowerFromMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        Camera cam = mainCamera != null ? mainCamera : Camera.main;
        if (cam == null)
            return;

        Ray ray = cam.ScreenPointToRay(mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, towerLayer.value))
        {
            ClearSelection();
            return;
        }

        TowerController tower = hit.collider.GetComponentInParent<TowerController>();
        if (tower == null)
        {
            ClearSelection();
            return;
        }

        SelectTower(tower);
    }

    public void SelectTower(TowerController tower)
    {
        selectedTower = tower;
        if (hud != null)
            hud.SetSelectedTower(tower);
    }

    public void ClearSelection()
    {
        selectedTower = null;
        if (hud != null)
            hud.ClearSelectedTower();
    }
}
