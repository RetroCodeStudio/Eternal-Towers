using UnityEngine;

namespace EternalTowers.Gameplay.Towers
{
    [System.Serializable]
    public class TowerLevelDefinition
    {
        [Header("Nivel")]
        public int level;

        [Header("Mejora")]
        public int upgradeCost = 50;

        [Header("Estadísticas")]
        public float damage = 10f;
        public float range = 3f;
        public float attackSpeed = 1f;

        [Header("Comportamiento")]
        public TowerBehaviorType behaviorType = TowerBehaviorType.Projectile;

        [Header("Proyectil")]
        public GameObject projectilePrefab;
        public float projectileSpeed = 10f;
        public float projectileLifeTime = 2f;

        [Header("Invocación")]
        public GameObject summonPrefab;
        public int summonCount = 1;
        public float summonCooldown = 6f;

        [Header("Visual")]
        public Sprite towerSprite;
    }
}
