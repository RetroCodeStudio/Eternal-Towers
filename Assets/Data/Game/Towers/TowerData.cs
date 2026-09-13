using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Eternal Towers/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("Identificación")]
    [SerializeField] private string towerId;
    [SerializeField] private string towerName;
    [SerializeField] private TowerType towerType;

    [Header("Estadísticas")]
    [SerializeField] private int cost;
    [SerializeField] private float damage;
    [SerializeField] private float range;
    [SerializeField] private float attackSpeed;

    public string TowerId => towerId;
    public string TowerName => towerName;
    public TowerType TowerType => towerType;
    public int Cost => cost;
    public float Damage => damage;
    public float Range => range;
    public float AttackSpeed => attackSpeed;
}