using System.Collections.Generic;
using UnityEngine;

namespace EternalTowers.Gameplay.Enemies
{
    public static class EnemyRouteRegistry
    {
        private static readonly Dictionary<int, EnemyRouteDefinition> Lanes = new Dictionary<int, EnemyRouteDefinition>();

        public static void RegisterLane(EnemyRouteDefinition route)
        {
            if (route == null || route.LaneId <= 0)
                return;

            foreach (KeyValuePair<int, EnemyRouteDefinition> entry in Lanes)
            {
                EnemyRouteDefinition registeredRoute = entry.Value;
                if (registeredRoute == null || registeredRoute == route)
                    continue;

                if (entry.Key == route.LaneId)
                {
                    Debug.LogWarning($"Lane ID {route.LaneId} is already assigned to '{registeredRoute.name}'. Each lane needs a unique ID.", route);
                    return;
                }

                if (route.Tilemap != null && registeredRoute.Tilemap == route.Tilemap)
                {
                    Debug.LogWarning($"Tilemap '{route.Tilemap.name}' is already assigned to Lane ID {entry.Key}.", route);
                    return;
                }
            }

            Lanes[route.LaneId] = route;
        }

        public static void UnregisterLane(EnemyRouteDefinition route)
        {
            if (route == null)
                return;

            List<int> routeIdsToRemove = new List<int>();
            foreach (KeyValuePair<int, EnemyRouteDefinition> entry in Lanes)
            {
                if (entry.Value == route)
                    routeIdsToRemove.Add(entry.Key);
            }

            for (int i = 0; i < routeIdsToRemove.Count; i++)
                Lanes.Remove(routeIdsToRemove[i]);
        }

        public static bool TryGetLane(int laneId, out EnemyRouteDefinition route)
        {
            route = null;
            if (laneId <= 0)
                return false;

            return Lanes.TryGetValue(laneId, out route);
        }

        public static bool HasLane(int laneId)
        {
            return laneId > 0 && Lanes.ContainsKey(laneId);
        }

        public static void Clear()
        {
            Lanes.Clear();
        }
    }
}
