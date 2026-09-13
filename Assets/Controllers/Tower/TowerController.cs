using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField] private TowerData towerData;

    public TowerData TowerData => towerData;

    public string TowerId => towerData != null ? towerData.TowerId : string.Empty;
    public string TowerName => towerData != null ? towerData.TowerName : string.Empty;
    public TowerType TowerType => towerData != null ? towerData.TowerType : default;

    public int Cost => towerData != null ? towerData.Cost : 0;
    public float Damage => towerData != null ? towerData.Damage : 0f;
    public float Range => towerData != null ? towerData.Range : 0f;
    public float AttackSpeed => towerData != null ? towerData.AttackSpeed : 0f;

    public void Initialize(TowerData data)
    {
        towerData = data;
    }
}