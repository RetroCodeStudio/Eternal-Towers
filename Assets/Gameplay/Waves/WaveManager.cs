using System;
using System.Collections;
using System.Collections.Generic;
using EternalTowers.Gameplay.Enemies;
using UnityEngine;

namespace EternalTowers.Gameplay.Waves
{
    public class WaveManager : MonoBehaviour
    {
        public enum WaveState
        {
            Waiting,
            Starting,
            Spawning,
            Active,
            Completed,
            Finished
        }

        private sealed class EnemyEventHandlers
        {
            public Action Died;
            public Action<int> ReachedBase;
            public Action<Enemy> SplitChildSpawned;
        }

        private sealed class WaveExecution
        {
            public readonly int WaveNumber;
            public readonly WaveDefinition Definition;
            public readonly HashSet<Enemy> TrackedEnemies = new HashSet<Enemy>();
            public readonly Dictionary<Enemy, EnemyEventHandlers> EventHandlers =
                new Dictionary<Enemy, EnemyEventHandlers>();
            public int NextSetIndex;
            public int ActiveEnemies;
            public bool SpawnScheduleComplete;

            public WaveExecution(int waveNumber, WaveDefinition definition)
            {
                WaveNumber = waveNumber;
                Definition = definition;
            }
        }

        public event Action<Enemy> EnemySpawned;
        public event Action<Enemy> EnemyDefeated;
        public event Action<Enemy> EnemyReachedBase;
        public event Action<int> WaveStarted;
        public event Action<int> WaveCompleted;

        [SerializeField] private List<WaveDefinition> waves = new List<WaveDefinition>();
        [SerializeField] private Transform spawnedEnemiesRoot;
        [SerializeField, Min(0f)] private float preWaveDelay = 1f;
        [SerializeField] private bool autoStartFirstWave = true;

        private readonly List<WaveExecution> runningWaves = new List<WaveExecution>();
        private int currentWaveIndex = -1;
        private int activeEnemies;
        private WaveExecution currentExecution;

        public int CurrentWaveIndex => currentWaveIndex;
        public WaveState State { get; private set; } = WaveState.Waiting;
        public int WaveCount => waves != null ? waves.Count : 0;
        public int ActiveEnemies => activeEnemies;
        public bool HasWave => currentWaveIndex >= 0 && currentWaveIndex < WaveCount;
        public bool CanStartNextWave => currentWaveIndex + 1 < WaveCount;

        private void Start()
        {
            if (autoStartFirstWave)
                StartNextWave();
        }

        private void OnDestroy()
        {
            for (int runIndex = 0; runIndex < runningWaves.Count; runIndex++)
            {
                WaveExecution execution = runningWaves[runIndex];
                foreach (KeyValuePair<Enemy, EnemyEventHandlers> entry in execution.EventHandlers)
                {
                    Enemy enemy = entry.Key;
                    EnemyEventHandlers handlers = entry.Value;
                    if (enemy == null)
                        continue;

                    enemy.Died -= handlers.Died;
                    enemy.BaseDamageRequested -= handlers.ReachedBase;
                    enemy.SplitChildSpawned -= handlers.SplitChildSpawned;
                }
            }

            runningWaves.Clear();
        }

        public void StartNextWave()
        {
            if (!CanStartNextWave)
            {
                State = WaveState.Finished;
                return;
            }

            currentWaveIndex++;
            currentExecution = new WaveExecution(currentWaveIndex + 1, waves[currentWaveIndex]);
            runningWaves.Add(currentExecution);
            State = WaveState.Starting;
            WaveStarted?.Invoke(currentExecution.WaveNumber);
            StartCoroutine(SpawnWaveRoutine(currentExecution));
        }

        public int GetPendingSetCount(int waveNumber)
        {
            for (int i = 0; i < runningWaves.Count; i++)
            {
                WaveExecution execution = runningWaves[i];
                if (execution.WaveNumber == waveNumber && execution.Definition != null)
                    return Mathf.Max(0, execution.Definition.EnemySets.Count - execution.NextSetIndex);
            }

            return 0;
        }

        private IEnumerator SpawnWaveRoutine(WaveExecution execution)
        {
            if (preWaveDelay > 0f)
                yield return new WaitForSeconds(preWaveDelay);

            if (execution == currentExecution)
                State = WaveState.Spawning;

            if (execution.Definition != null)
            {
                IReadOnlyList<WaveEnemySet> sets = execution.Definition.EnemySets;
                for (int setIndex = 0; setIndex < sets.Count; setIndex++)
                {
                    WaveEnemySet set = sets[setIndex];
                    execution.NextSetIndex = setIndex + 1;
                    if (set == null)
                        continue;

                    if (set.delayBeforeSet > 0f)
                        yield return new WaitForSeconds(set.delayBeforeSet);

                    if (!TryGetLanes(set.laneIds, execution.WaveNumber, setIndex, out List<EnemyRouteDefinition> lanes))
                        continue;

                    IReadOnlyList<WaveEnemyGroup> groups = set.enemyGroups;
                    if (groups == null)
                        continue;

                    List<int> spawnOrder = BuildRandomSpawnOrder(set, execution.WaveNumber, setIndex);
                    List<int> lanePattern = BuildRandomLanePattern(set, spawnOrder.Count, execution.WaveNumber, setIndex);
                    for (int spawnIndex = 0; spawnIndex < spawnOrder.Count; spawnIndex++)
                    {
                        WaveEnemyGroup group = groups[spawnOrder[spawnIndex]];
                        int laneId = lanePattern.Count > spawnIndex ? lanePattern[spawnIndex] : 0;
                        SpawnEnemy(execution, group.enemyPrefab, lanes, laneId);

                        if (spawnIndex + 1 < spawnOrder.Count)
                            yield return new WaitForSeconds(Mathf.Max(0.05f, set.spawnInterval));
                    }
                }
            }

            execution.SpawnScheduleComplete = true;
            TryCompleteWave(execution);
        }

        private bool TryGetLanes(
            IReadOnlyList<int> laneIds,
            int waveNumber,
            int setIndex,
            out List<EnemyRouteDefinition> lanes)
        {
            lanes = new List<EnemyRouteDefinition>();
            if (laneIds == null || laneIds.Count == 0)
            {
                Debug.LogWarning($"Wave {waveNumber}, set {setIndex + 1}: no Lane IDs are assigned.", this);
                return false;
            }

            for (int laneIndex = 0; laneIndex < laneIds.Count; laneIndex++)
            {
                int laneId = laneIds[laneIndex];
                if (!EnemyRouteRegistry.TryGetLane(laneId, out EnemyRouteDefinition lane))
                {
                    Debug.LogWarning($"Wave {waveNumber}, set {setIndex + 1}: Lane ID {laneId} is not registered.", this);
                    continue;
                }

                EnemyRoutePathData pathData = EnemyRoutePathfinder.GetOrCreate(lane);
                if (pathData == null || !pathData.IsValid || lane.Entry == null)
                {
                    Debug.LogWarning($"Wave {waveNumber}, set {setIndex + 1}: Lane ID {laneId} has no valid path or Entry.", this);
                    continue;
                }

                if (!lanes.Contains(lane))
                    lanes.Add(lane);
            }

            return lanes.Count > 0;
        }

        private static List<int> BuildRandomSpawnOrder(WaveEnemySet set, int waveNumber, int setNumber)
        {
            IReadOnlyList<WaveEnemyGroup> groups = set.enemyGroups;
            int remainingEnemyCount = 0;

            for (int groupIndex = 0; groupIndex < groups.Count; groupIndex++)
            {
                WaveEnemyGroup group = groups[groupIndex];
                if (group == null || group.enemyPrefab == null || group.count <= 0)
                {
                    Debug.LogWarning($"Wave {waveNumber}, set {setNumber + 1}: enemy group {groupIndex + 1} is invalid and will be skipped.");
                    continue;
                }

                remainingEnemyCount += group.count;
            }

            List<int> shuffledOrder = new List<int>(remainingEnemyCount);
            for (int groupIndex = 0; groupIndex < groups.Count; groupIndex++)
            {
                WaveEnemyGroup group = groups[groupIndex];
                if (group == null || group.enemyPrefab == null || group.count <= 0)
                    continue;

                for (int count = 0; count < group.count; count++)
                    shuffledOrder.Add(groupIndex);
            }

            for (int index = shuffledOrder.Count - 1; index > 0; index--)
            {
                int swapIndex = UnityEngine.Random.Range(0, index + 1);
                int currentGroup = shuffledOrder[index];
                shuffledOrder[index] = shuffledOrder[swapIndex];
                shuffledOrder[swapIndex] = currentGroup;
            }

            return shuffledOrder;
        }

        private static List<int> BuildRandomLanePattern(
            WaveEnemySet set,
            int enemyCount,
            int waveNumber,
            int setNumber)
        {
            if (set == null || set.laneIds == null || set.laneIds.Count == 0)
                return new List<int>();

            if (set.spawnPatterns != null && set.spawnPatterns.Count > 0)
            {
                List<WaveSpawnPattern> validPatterns = new List<WaveSpawnPattern>();
                for (int patternIndex = 0; patternIndex < set.spawnPatterns.Count; patternIndex++)
                {
                    WaveSpawnPattern pattern = set.spawnPatterns[patternIndex];
                    if (pattern == null || pattern.laneIds == null || pattern.laneIds.Count == 0)
                        continue;

                    if (ContainsOnlyAllowedLanes(pattern.laneIds, set.laneIds))
                        validPatterns.Add(pattern);
                }

                if (validPatterns.Count > 0)
                {
                    WaveSpawnPattern selectedPattern = validPatterns[UnityEngine.Random.Range(0, validPatterns.Count)];
                    List<int> patternLaneOrder = new List<int>(enemyCount);
                    for (int enemyIndex = 0; enemyIndex < enemyCount; enemyIndex++)
                        patternLaneOrder.Add(selectedPattern.laneIds[enemyIndex % selectedPattern.laneIds.Count]);

                    return patternLaneOrder;
                }
            }

            List<int> fallbackPattern = new List<int>(enemyCount);
            for (int enemyIndex = 0; enemyIndex < enemyCount; enemyIndex++)
                fallbackPattern.Add(set.laneIds[UnityEngine.Random.Range(0, set.laneIds.Count)]);

            return fallbackPattern;
        }

        private static bool ContainsOnlyAllowedLanes(IReadOnlyList<int> patternLaneIds, IReadOnlyList<int> allowedLaneIds)
        {
            if (patternLaneIds == null || patternLaneIds.Count == 0)
                return false;

            for (int patternIndex = 0; patternIndex < patternLaneIds.Count; patternIndex++)
            {
                int laneId = patternLaneIds[patternIndex];
                bool found = false;
                for (int allowedIndex = 0; allowedIndex < allowedLaneIds.Count; allowedIndex++)
                {
                    if (allowedLaneIds[allowedIndex] == laneId)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    return false;
            }

            return true;
        }

        private void SpawnEnemy(
            WaveExecution execution,
            Enemy prefab,
            IReadOnlyList<EnemyRouteDefinition> availableLanes,
            int laneId)
        {
            if (prefab == null || availableLanes == null || availableLanes.Count == 0)
                return;

            EnemyRouteDefinition lane = null;
            for (int index = 0; index < availableLanes.Count; index++)
            {
                EnemyRouteDefinition candidate = availableLanes[index];
                if (candidate != null && candidate.LaneId == laneId)
                {
                    lane = candidate;
                    break;
                }
            }

            if (lane == null)
            {
                int selectedLaneIndex = UnityEngine.Random.Range(0, availableLanes.Count);
                lane = availableLanes[selectedLaneIndex];
            }

            Enemy enemy = Instantiate(prefab, lane.Entry.position, lane.Entry.rotation, spawnedEnemiesRoot);
            enemy.BeginRouteMovement(lane);
            TrackEnemy(execution, enemy);
        }

        private void TrackEnemy(WaveExecution execution, Enemy enemy)
        {
            if (enemy == null || !execution.TrackedEnemies.Add(enemy))
                return;

            execution.ActiveEnemies++;
            activeEnemies++;

            EnemyEventHandlers handlers = new EnemyEventHandlers();
            handlers.Died = () => HandleEnemyDefeated(execution, enemy);
            handlers.ReachedBase = _ => HandleEnemyReachedBase(execution, enemy);
            handlers.SplitChildSpawned = child => TrackEnemy(execution, child);
            execution.EventHandlers.Add(enemy, handlers);

            enemy.Died += handlers.Died;
            enemy.BaseDamageRequested += handlers.ReachedBase;
            enemy.SplitChildSpawned += handlers.SplitChildSpawned;
            EnemySpawned?.Invoke(enemy);
        }

        private void HandleEnemyDefeated(WaveExecution execution, Enemy enemy)
        {
            if (!UntrackEnemy(execution, enemy))
                return;

            EnemyDefeated?.Invoke(enemy);
            TryCompleteWave(execution);
        }

        private void HandleEnemyReachedBase(WaveExecution execution, Enemy enemy)
        {
            if (!UntrackEnemy(execution, enemy))
                return;

            EnemyReachedBase?.Invoke(enemy);
            TryCompleteWave(execution);
        }

        private bool UntrackEnemy(WaveExecution execution, Enemy enemy)
        {
            if (!execution.TrackedEnemies.Remove(enemy))
                return false;

            execution.ActiveEnemies = Mathf.Max(0, execution.ActiveEnemies - 1);
            activeEnemies = Mathf.Max(0, activeEnemies - 1);

            if (execution.EventHandlers.TryGetValue(enemy, out EnemyEventHandlers handlers))
            {
                enemy.Died -= handlers.Died;
                enemy.BaseDamageRequested -= handlers.ReachedBase;
                enemy.SplitChildSpawned -= handlers.SplitChildSpawned;
                execution.EventHandlers.Remove(enemy);
            }

            return true;
        }

        private void TryCompleteWave(WaveExecution execution)
        {
            if (!execution.SpawnScheduleComplete || execution.ActiveEnemies > 0)
            {
                if (execution == currentExecution && execution.SpawnScheduleComplete)
                    State = WaveState.Active;
                return;
            }

            runningWaves.Remove(execution);
            if (execution == currentExecution)
                State = WaveState.Completed;

            WaveCompleted?.Invoke(execution.WaveNumber);
        }
    }
}