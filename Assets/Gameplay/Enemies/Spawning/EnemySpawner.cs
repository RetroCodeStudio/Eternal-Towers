using UnityEngine;

namespace EternalTowers.Gameplay.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Enemy enemyPrefab;
        [SerializeField] private EnemyPath path;
        [SerializeField] private Transform spawnPoint;
        [SerializeField, Min(1)] private int laneCount = 3;
        [SerializeField, Min(0f)] private float laneSpacing = 0.75f;

        private int nextLaneIndex;

        public Enemy Spawn()
        {
            if (enemyPrefab == null)
            {
                Debug.LogWarning("EnemySpawner: enemyPrefab is null.", this);
                return null;
            }

            if (path == null)
            {
                Debug.LogWarning("EnemySpawner: no path assigned.", this);
                return null;
            }

            if (path.WaypointCount <= 0)
            {
                Debug.LogWarning($"EnemySpawner: path '{path.name}' has no waypoints.", this);
                return null;
            }

            Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
            Quaternion spawnRotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;
            Enemy enemy = Instantiate(enemyPrefab, spawnPosition, spawnRotation);

            float laneOffset = GetNextLaneOffset();
            Debug.Log($"EnemySpawner: spawning '{enemy.name}' at {spawnPosition}. path={path.name}. laneOffset={laneOffset}.", this);
            enemy.SetPath(path);
            enemy.SetLaneOffset(laneOffset);
            enemy.BeginMovement();
            return enemy;
        }

        private float GetNextLaneOffset()
        {
            int centeredIndex = nextLaneIndex % laneCount;
            nextLaneIndex++;
            return (centeredIndex - (laneCount - 1) * 0.5f) * laneSpacing;
        }
    }
}