using UnityEngine;

public class TowerPlacementInput : MonoBehaviour
{
    [HideInInspector]
    [SerializeField] private TowerPlacementSystem placementSystem;
    [HideInInspector]
    [SerializeField] private GameObject towerPrefab;
    [HideInInspector]
    [SerializeField] private TowerData towerData;
    [HideInInspector]
    [SerializeField] private KeyCode placeKey = KeyCode.Mouse0;
}
