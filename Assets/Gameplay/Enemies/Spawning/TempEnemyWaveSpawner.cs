using System.Collections;
using UnityEngine;

namespace EternalTowers.Gameplay.Enemies
{
    public class TempEnemyWaveSpawner : MonoBehaviour
    {
        [SerializeField] private EnemySpawner spawner;
        [SerializeField] private float initialDelay = 0.5f;
        [SerializeField] private float intervalBetweenSpawns = 0.75f;

        private void Start()
        {
            if (spawner == null)
                spawner = GetComponent<EnemySpawner>();

            if (spawner == null)
            {
                Debug.LogWarning("TempEnemyWaveSpawner: no EnemySpawner assigned or found on this GameObject.");
                return;
            }

            StartCoroutine(SpawnLoop());
        }

        [ContextMenu("Spawn One Enemy")]
        public void SpawnOne()
        {
            if (spawner != null)
                spawner.Spawn();
        }

        private IEnumerator SpawnLoop()
        {
            yield return new WaitForSeconds(initialDelay);

            while (true)
            {
                spawner.Spawn();
                yield return new WaitForSeconds(intervalBetweenSpawns);
            }
        }
    }
}
