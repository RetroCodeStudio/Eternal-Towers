using UnityEngine;

namespace EternalTowers.Gameplay.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Enemy enemyPrefab;
        [SerializeField] private EnemyPath path;
        [SerializeField] private Transform spawnPoint;

        public Enemy Spawn()
        {
            if (enemyPrefab == null)
                return null;

            Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
            Quaternion spawnRotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;
            Enemy enemy = Instantiate(enemyPrefab, spawnPosition, spawnRotation);

            if (path != null)
                enemy.SetPath(path);

            enemy.BeginMovement();
            return enemy;
        }
    }
}