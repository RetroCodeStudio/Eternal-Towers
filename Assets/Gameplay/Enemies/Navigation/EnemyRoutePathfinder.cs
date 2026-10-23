using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
//Pathfinding for enemy routes using a tilemap-based approach. This class calculates and caches valid paths for enemies to follow based on defined entry and exit points, ensuring that the paths are navigable and meet lane requirements.
namespace EternalTowers.Gameplay.Enemies
{
    public static class EnemyRoutePathfinder
    {
        private static readonly Dictionary<EnemyRouteDefinition, EnemyRoutePathData> LanePathCache = new Dictionary<EnemyRouteDefinition, EnemyRoutePathData>();

        public static EnemyRoutePathData CalculateRoute(EnemyRouteDefinition route)
        {
            if (TryCalculateRoute(route, out EnemyRoutePathData path, out string error))
                return path;

            Debug.LogWarning(error);
            return null;
        }

        public static bool TryCalculateRoute(EnemyRouteDefinition route, out EnemyRoutePathData path, out string error)
        {
            path = null;
            error = string.Empty;

            if (route == null)
            {
                error = "Route is null.";
                return false;
            }

            if (route.LaneId <= 0)
            {
                error = "Lane ID must be greater than zero.";
                return false;
            }

            if (route.Tilemap == null)
            {
                error = $"[Lane {route.LaneId}] Tilemap is missing.";
                return false;
            }

            if (route.Entry == null)
            {
                error = $"[Lane {route.LaneId}] Entry is missing.";
                return false;
            }

            if (route.Exit == null)
            {
                error = $"[Lane {route.LaneId}] Exit is missing.";
                return false;
            }

            Vector3Int startCell = route.Tilemap.WorldToCell(route.Entry.position);
            Vector3Int endCell = route.Tilemap.WorldToCell(route.Exit.position);

            if (!route.Tilemap.HasTile(startCell))
            {
                error = $"[Lane {route.LaneId}] Entry is not inside the lane Tilemap.";
                return false;
            }

            if (!route.Tilemap.HasTile(endCell))
            {
                error = $"[Lane {route.LaneId}] Exit is not inside the lane Tilemap.";
                return false;
            }

            if (!TryBuildCellPath(route.Tilemap, startCell, endCell, out List<Vector3Int> cells, out string buildError))
            {
                error = $"[Lane {route.LaneId}] {buildError}";
                return false;
            }

            path = BuildRouteData(route.Tilemap, cells);

            if (!path.IsValid)
            {
                error = $"[Lane {route.LaneId}] no valid path was generated between Entry and Exit.";
                return false;
            }

            LanePathCache[route] = path;
            return true;
        }

        public static EnemyRoutePathData GetOrCreate(EnemyRouteDefinition route)
        {
            if (route == null)
                return null;

            if (LanePathCache.TryGetValue(route, out EnemyRoutePathData cachedPath) && cachedPath != null)
                return cachedPath;

            return CalculateRoute(route);
        }

        public static void Invalidate(int laneId)
        {
            if (laneId <= 0)
                return;

            List<EnemyRouteDefinition> routesToRemove = new List<EnemyRouteDefinition>();
            foreach (EnemyRouteDefinition route in LanePathCache.Keys)
            {
                if (route != null && route.LaneId == laneId)
                    routesToRemove.Add(route);
            }

            for (int i = 0; i < routesToRemove.Count; i++)
                LanePathCache.Remove(routesToRemove[i]);
        }

        public static void Invalidate(EnemyRouteDefinition route)
        {
            if (route != null)
                LanePathCache.Remove(route);
        }

        private static EnemyRoutePathData BuildRouteData(
            Tilemap tilemap,
            List<Vector3Int> cells)
        {
            EnemyRoutePathData routeData = new EnemyRoutePathData
            {
                Tilemap = tilemap
            };

            routeData.Cells.AddRange(cells);
            for (int i = 0; i < cells.Count; i++)
            {
                routeData.Points.Add(tilemap.GetCellCenterWorld(cells[i]));
            }

            return routeData;
        }

        private static bool TryBuildCellPath(Tilemap tilemap, Vector3Int startCell, Vector3Int endCell, out List<Vector3Int> cells, out string error)
        {
            cells = new List<Vector3Int>();
            error = string.Empty;

            if (tilemap == null)
            {
                error = "Tilemap is null.";
                return false;
            }

            if (!tilemap.HasTile(startCell) || !tilemap.HasTile(endCell))
            {
                error = "Entry or Exit is not on a walkable Tilemap cell.";
                return false;
            }

            if (startCell == endCell)
            {
                cells.Add(startCell);
                return true;
            }

            Queue<Vector3Int> frontier = new Queue<Vector3Int>();
            Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
            HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
            Vector3Int[] directions = { Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left };

            frontier.Enqueue(startCell);
            visited.Add(startCell);

            bool foundPath = false;
            while (frontier.Count > 0)
            {
                Vector3Int current = frontier.Dequeue();
                if (current == endCell)
                {
                    foundPath = true;
                    break;
                }

                for (int directionIndex = 0; directionIndex < directions.Length; directionIndex++)
                {
                    Vector3Int neighbor = current + directions[directionIndex];
                    if (!tilemap.HasTile(neighbor) || visited.Contains(neighbor))
                        continue;

                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                    frontier.Enqueue(neighbor);
                }
            }

            if (!foundPath)
            {
                error = "No valid route exists between Entry and Exit.";
                return false;
            }

            Vector3Int cursor = endCell;
            while (cursor != startCell)
            {
                cells.Add(cursor);
                if (!cameFrom.TryGetValue(cursor, out Vector3Int previous))
                    break;

                cursor = previous;
            }

            cells.Add(startCell);
            cells.Reverse();
            return cells.Count > 1;
        }
    }
}
