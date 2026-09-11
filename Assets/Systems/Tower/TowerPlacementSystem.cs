using UnityEngine;

public class TowerPlacementSystem : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform towerParent;

    [Header("Validación")]
    [SerializeField] private Collider2D placementArea;
    [SerializeField] private LayerMask blockedAreaMask;

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

    public GameObject PlaceTower(GameObject towerPrefab, Vector2 position)
    {
        if (towerPrefab == null)
        {
            return null;
        }

        if (!CanPlaceTower(position))
        {
            return null;
        }

        return Instantiate(towerPrefab, position, Quaternion.identity, towerParent);
    }
}