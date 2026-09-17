using System.Collections.Generic;
using UnityEngine;

namespace EternalTowers.Gameplay.Enemies
{
    public class EnemyPath : MonoBehaviour
    {
        [SerializeField] private List<Transform> waypoints = new List<Transform>();
        [SerializeField, Min(0f)] private float pathWidth = 3f;

        public int WaypointCount => waypoints.Count;
        public float PathWidth => pathWidth;

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

        public Vector3 GetWaypointPosition(int index, float laneOffset)
        {
            Transform waypoint = GetWaypoint(index);
            if (waypoint == null)
            {
                Debug.LogWarning($"EnemyPath[{name}]: requested waypoint index {index} is invalid. Waypoint count={waypoints.Count}.", this);
                return Vector3.zero;
            }

            if (Mathf.Approximately(laneOffset, 0f))
                return waypoint.position;

            Vector3 direction = GetDirection(index);
            Vector3 lateral = new Vector3(-direction.y, direction.x, 0f);
            return waypoint.position + lateral * laneOffset;
        }

        private Vector3 GetDirection(int index)
        {
            Transform current = GetWaypoint(index);
            Transform next = GetWaypoint(index + 1);
            Transform previous = GetWaypoint(index - 1);

            Vector3 direction = next != null && current != null
                ? next.position - current.position
                : current != null && previous != null
                    ? current.position - previous.position
                    : Vector3.right;

            direction.z = 0f;
            return direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector3.right;
        }

        private void OnDrawGizmos()
        {
            DrawPath(Color.yellow, false);
        }

        private void OnDrawGizmosSelected()
        {
            DrawPath(Color.cyan, true);
        }

        private void DrawPath(Color pathColor, bool drawWidth)
        {
            Gizmos.color = pathColor;

            for (int index = 0; index < waypoints.Count; index++)
            {
                Transform waypoint = waypoints[index];
                if (waypoint == null)
                    continue;

                Gizmos.DrawSphere(waypoint.position, 0.16f);

                if (index > 0 && waypoints[index - 1] != null)
                {
                    Gizmos.DrawLine(waypoints[index - 1].position, waypoint.position);

                    if (drawWidth && pathWidth > 0f)
                        DrawPathEdges(waypoints[index - 1].position, waypoint.position);
                }
            }
        }

        private void DrawPathEdges(Vector3 start, Vector3 end)
        {
            Vector3 direction = end - start;
            direction.z = 0f;

            if (direction.sqrMagnitude <= 0.0001f)
                return;

            Vector3 lateral = new Vector3(-direction.normalized.y, direction.normalized.x, 0f);
            float halfWidth = pathWidth * 0.5f;
            Gizmos.DrawLine(start + lateral * halfWidth, end + lateral * halfWidth);
            Gizmos.DrawLine(start - lateral * halfWidth, end - lateral * halfWidth);
        }
    }
}