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
        private int currentWaypointIndex;
        private bool isMoving;
        private float speedMultiplier = 1f;
        private int speedBoostVersion;

        public int CurrentWaypointIndex => currentWaypointIndex;
        public bool IsMoving => isMoving;
        public float CurrentSpeed => moveSpeed * speedMultiplier;

        public void Begin(EnemyPath enemyPath, float speed)
        {
            path = enemyPath;
            moveSpeed = Mathf.Max(0f, speed);
            currentWaypointIndex = 0;
            isMoving = path != null && path.WaypointCount > 0;
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
                StartCoroutine(RemoveSpeedMultiplierAfter(duration, speedBoostVersion));
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

            Transform waypoint = path.GetWaypoint(currentWaypointIndex);
            if (waypoint == null)
            {
                AdvanceToNextWaypoint();
                return;
            }

            Vector3 offset = waypoint.position - transform.position;
            if (offset.sqrMagnitude <= 0.0001f)
            {
                AdvanceToNextWaypoint();
                return;
            }

            DirectionChanged?.Invoke(offset);
            transform.position = Vector3.MoveTowards(
                transform.position,
                waypoint.position,
                CurrentSpeed * deltaTime);
        }

        private IEnumerator RemoveSpeedMultiplierAfter(float duration, int version)
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