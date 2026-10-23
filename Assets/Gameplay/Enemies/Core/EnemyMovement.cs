using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EternalTowers.Gameplay.Enemies
{
    public class EnemyMovement : MonoBehaviour
    {
        public event Action<Vector3> DirectionChanged;
        public event Action ReachedGoal;

        private EnemyRouteDefinition routeDefinition;
        private IReadOnlyList<Vector3> routePoints;
        private float moveSpeed;
        private int currentRouteIndex;
        private bool isMoving;
        private float speedMultiplier = 1f;
        private int speedBoostVersion;

        public int CurrentRouteIndex => currentRouteIndex;
        public bool IsMoving => isMoving;
        public float CurrentSpeed => moveSpeed * speedMultiplier;
        public float PathProgress
        {
            get
            {
                if (routeDefinition == null || routePoints == null || routePoints.Count == 0)
                    return 0f;

                if (currentRouteIndex >= routePoints.Count)
                    return 1f;

                Vector3 previousPoint = routePoints[Mathf.Max(0, currentRouteIndex - 1)];
                float progress = GetSegmentProgress(previousPoint, routePoints[currentRouteIndex]);
                return Mathf.Clamp01((currentRouteIndex + progress) / routePoints.Count);
            }
        }

        public void BeginRoute(EnemyRouteDefinition enemyRoute, float speed)
        {
            routeDefinition = enemyRoute;
            routePoints = null;
            moveSpeed = Mathf.Max(0f, speed);
            currentRouteIndex = 0;

            if (routeDefinition == null)
            {
                isMoving = false;
                return;
            }

            EnemyRoutePathData routeData = EnemyRoutePathfinder.GetOrCreate(routeDefinition);
            if (routeData == null || !routeData.IsValid)
            {
                isMoving = false;
                return;
            }

            routePoints = routeData.Points;
            if (routePoints.Count > 0)
            {
                if (Vector3.Distance(transform.position, routePoints[0]) < 0.05f && routePoints.Count > 1)
                    currentRouteIndex = 1;
            }

            isMoving = true;
        }

        public void CopyNavigationTo(EnemyMovement target, float targetSpeed)
        {
            if (target == null)
                return;

            target.routeDefinition = routeDefinition;
            target.routePoints = routePoints;
            target.moveSpeed = Mathf.Max(0f, targetSpeed);
            target.currentRouteIndex = currentRouteIndex;
            target.speedMultiplier = speedMultiplier;
            target.isMoving = isMoving;
        }

        public void Stop()
        {
            isMoving = false;
        }

        public void ResetNavigation()
        {
            routeDefinition = null;
            routePoints = null;
            moveSpeed = 0f;
            currentRouteIndex = 0;
            isMoving = false;
            speedMultiplier = 1f;
            speedBoostVersion++;
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

            if (routeDefinition == null || routePoints == null || routePoints.Count == 0)
            {
                isMoving = false;
                return;
            }

            TickRoute(deltaTime);
        }

        private void TickRoute(float deltaTime)
        {
            if (routePoints.Count == 0)
            {
                isMoving = false;
                return;
            }

            if (currentRouteIndex >= routePoints.Count)
            {
                isMoving = false;
                ReachedGoal?.Invoke();
                return;
            }

            Vector3 targetPosition = routePoints[currentRouteIndex];
            Vector3 direction = targetPosition - transform.position;

            if (direction.sqrMagnitude <= 0.0001f)
            {
                currentRouteIndex++;
                return;
            }

            DirectionChanged?.Invoke(direction);

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                CurrentSpeed * deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                currentRouteIndex++;
                if (currentRouteIndex >= routePoints.Count)
                {
                    isMoving = false;
                    ReachedGoal?.Invoke();
                }
            }
        }

        private float GetSegmentProgress(Vector3 previousPoint, Vector3 targetPoint)
        {
            float segmentLength = Vector3.Distance(previousPoint, targetPoint);
            if (segmentLength <= 0f)
                return 0f;

            return Mathf.Clamp01(1f - Vector3.Distance(transform.position, targetPoint) / segmentLength);
        }

        private IEnumerator RemoveSpeedMultiplierAfter(
            float duration,
            int version)
        {
            yield return new WaitForSeconds(duration);

            if (version == speedBoostVersion)
                speedMultiplier = 1f;
        }

    }
}