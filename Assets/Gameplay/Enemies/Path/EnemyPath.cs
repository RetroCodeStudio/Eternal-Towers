using System.Collections.Generic;
using UnityEngine;

namespace EternalTowers.Gameplay.Enemies
{
    public class EnemyPath : MonoBehaviour
    {
        [SerializeField] private List<Transform> waypoints = new List<Transform>();

        public int WaypointCount => waypoints.Count;

        public Transform GetWaypoint(int index)
        {
            if (index < 0 || index >= waypoints.Count)
                return null;

            return waypoints[index];
        }

        public void SetWaypoints(params Transform[] newWaypoints)
        {
            waypoints.Clear();

            if (newWaypoints != null)
                waypoints.AddRange(newWaypoints);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            for (int index = 0; index < waypoints.Count; index++)
            {
                Transform waypoint = waypoints[index];
                if (waypoint == null)
                    continue;

                Gizmos.DrawSphere(waypoint.position, 0.12f);

                if (index > 0 && waypoints[index - 1] != null)
                    Gizmos.DrawLine(waypoints[index - 1].position, waypoint.position);
            }
        }
    }
}