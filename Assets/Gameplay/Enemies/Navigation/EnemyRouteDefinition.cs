using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
//hola
namespace EternalTowers.Gameplay.Enemies
{
    public struct RouteValidationResult
    {
        public bool success;
        public string message;

        public static RouteValidationResult Valid => new RouteValidationResult
        {
            success = true,
            message = string.Empty
        };
    }

    public sealed class EnemyRoutePathData
    {
        public Tilemap Tilemap { get; internal set; }
        public readonly List<Vector3Int> Cells = new List<Vector3Int>();
        public readonly List<Vector3> Points = new List<Vector3>();
        public bool IsValid => Points != null && Points.Count > 1;
    }

    [DisallowMultipleComponent]
    public class EnemyRouteDefinition : MonoBehaviour
    {
        [FormerlySerializedAs("routeId"), SerializeField, Min(1)] private int laneId = 1;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Transform entry;
        [SerializeField] private Transform exit;

        public int LaneId => laneId;
        public Tilemap Tilemap => tilemap;
        public Transform Entry => entry;
        public Transform Exit => exit;

        private void OnEnable()
        {
            Register();
        }

        private void OnDisable()
        {
            EnemyRouteRegistry.UnregisterLane(this);
        }

        private void OnDestroy()
        {
            EnemyRouteRegistry.UnregisterLane(this);
        }

        public void Initialize(int newLaneId, Tilemap newTilemap, Transform newEntry, Transform newExit)
        {
            EnemyRouteRegistry.UnregisterLane(this);
            EnemyRoutePathfinder.Invalidate(this);
            laneId = newLaneId;
            tilemap = newTilemap;
            entry = newEntry;
            exit = newExit;
            Register();
        }

        public void Register()
        {
            if (laneId > 0)
                EnemyRouteRegistry.RegisterLane(this);
        }

        public RouteValidationResult Validate()
        {
            if (laneId <= 0)
                return new RouteValidationResult { success = false, message = "Lane ID must be greater than zero." };

            if (tilemap == null)
                return new RouteValidationResult { success = false, message = $"[Lane {laneId}] Tilemap is missing." };

            if (entry == null)
                return new RouteValidationResult { success = false, message = $"[Lane {laneId}] Entry is missing." };

            if (exit == null)
                return new RouteValidationResult { success = false, message = $"[Lane {laneId}] Exit is missing." };

            Vector3Int entryCell = tilemap.WorldToCell(entry.position);
            Vector3Int exitCell = tilemap.WorldToCell(exit.position);

            if (!tilemap.HasTile(entryCell))
                return new RouteValidationResult { success = false, message = $"[Lane {laneId}] Entry is outside the walkable Tilemap." };

            if (!tilemap.HasTile(exitCell))
                return new RouteValidationResult { success = false, message = $"[Lane {laneId}] Exit is outside the walkable Tilemap." };

            if (EnemyRoutePathfinder.TryCalculateRoute(this, out _, out string error))
                return RouteValidationResult.Valid;

            return new RouteValidationResult { success = false, message = $"[Lane {laneId}] {error}" };
        }

        public EnemyRoutePathData GetPathData()
        {
            return EnemyRoutePathfinder.GetOrCreate(this);
        }

        public void InvalidatePath()
        {
            EnemyRoutePathfinder.Invalidate(this);
        }

        private void OnValidate()
        {
            EnemyRoutePathfinder.Invalidate(this);
            EnemyRouteRegistry.UnregisterLane(this);
            if (isActiveAndEnabled)
                Register();
        }
    }
}
