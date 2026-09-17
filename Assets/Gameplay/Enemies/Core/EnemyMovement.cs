using System;
using System.Collections;
using UnityEngine;

namespace EternalTowers.Gameplay.Enemies
{
    public class EnemyMovement : MonoBehaviour
    {
        public event Action<Vector3> DirectionChanged;
        public event Action ReachedGoal;

        private EnemyPath path;
        private float moveSpeed;
        private float laneOffset;
        private int currentWaypointIndex;
        private bool isMoving;
        private float speedMultiplier = 1f;
        private int speedBoostVersion;

        public int CurrentWaypointIndex => currentWaypointIndex;
        public bool IsMoving => isMoving;
        public float CurrentSpeed => moveSpeed * speedMultiplier;

        public void Begin(EnemyPath enemyPath, float speed)
        {
            Begin(enemyPath, speed, 0f);
        }

        public void Begin(EnemyPath enemyPath, float speed, float offset)
        {
            path = enemyPath;
            moveSpeed = Mathf.Max(0f, speed);
            laneOffset = offset;
            currentWaypointIndex = 0;

            if (path == null || path.WaypointCount <= 0)
            {
                isMoving = false;
                return;
            }

            if (path.GetWaypoint(0) != null)
            {
                Vector3 firstWaypointPosition =
                    path.GetWaypointPosition(0, laneOffset);

                if (Vector3.Distance(
                        transform.position,
                        firstWaypointPosition) < 0.05f
                    && path.WaypointCount > 1)
                {
                    currentWaypointIndex = 1;
                }
            }

            isMoving = true;
        }

        public void Stop()
        {
            isMoving = false;
        }

        public void ApplySpeedMultiplier(float multiplier, float duration)
        {
            speedMultiplier = Mathf.Max(speedMultiplier, multiplier);
            speedBoostVersion++;

            if (duration > 0f && Application.isPlaying)
            {
                StartCoroutine(
                    RemoveSpeedMultiplierAfter(
                        duration,
                        speedBoostVersion
                    )
                );
            }
        }

        private void Update()
        {
            if (!isMoving)
                return;

            Tick(Time.deltaTime);
        }

        internal void Tick(float deltaTime)
        {
            if (!isMoving)
                return;

            if (path == null || path.WaypointCount <= 0)
            {
                isMoving = false;
                return;
            }

            Transform waypoint =
                path.GetWaypoint(currentWaypointIndex);

            if (waypoint == null)
            {
                AdvanceToNextWaypoint();
                return;
            }

            Vector3 targetPosition =
                path.GetWaypointPosition(
                    currentWaypointIndex,
                    laneOffset
                );

            Vector3 direction =
                targetPosition - transform.position;

            if (direction.sqrMagnitude <= 0.0001f)
            {
                AdvanceToNextWaypoint();
                return;
            }

            DirectionChanged?.Invoke(direction);

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                CurrentSpeed * deltaTime
            );
        }

        private IEnumerator RemoveSpeedMultiplierAfter(
            float duration,
            int version)
        {
            yield return new WaitForSeconds(duration);

            if (version == speedBoostVersion)
                speedMultiplier = 1f;
        }

        private void AdvanceToNextWaypoint()
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= path.WaypointCount)
            {
                isMoving = false;
                ReachedGoal?.Invoke();
            }
        }
    }
}