using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EternalTowers.Gameplay.Enemies
{
    [RequireComponent(typeof(EnemyMovement))]
    public class Enemy : MonoBehaviour
    {
        [Serializable]
        public class IntEvent : UnityEvent<int>
        {
        }

        [Header("Stats")]
        [SerializeField, Min(1f)] private float maxHealth = 10f;
        [SerializeField, Min(0f)] private float moveSpeed = 1f;
        [SerializeField, Min(0)] private int baseDamage = 1;
        [SerializeField, Min(0)] private int reward;

        [Header("Movement")]
        [SerializeField] private EnemyPath path;
        [SerializeField] private EnemyMovement movement;
        [SerializeField] private float laneOffset;

        [Header("Visual")]
        [SerializeField] private EnemyVisual visual;
        [SerializeField, Min(0f)] private float deathAnimationDuration = 1f;

        [Header("Events")]
        [SerializeField] private IntEvent onBaseDamageRequested = new IntEvent();
        [SerializeField] private IntEvent onRewardGranted = new IntEvent();

        public event Action<int> BaseDamageRequested;
        public event Action<int> RewardGranted;
        public event Action Died;

        public float MaxHealth => maxHealth;
        public float CurrentHealth { get; private set; }
        public float MoveSpeed => moveSpeed;
        public int BaseDamage => baseDamage;
        public int Reward => reward;
        public EnemyState State { get; private set; } = EnemyState.Spawning;
        public EnemyPath Path => path;
        public float LaneOffset => laneOffset;

        private readonly Dictionary<object, float> damageReductionSources =
            new Dictionary<object, float>();

        private void Awake()
        {
            if (movement == null)
                movement = GetComponent<EnemyMovement>();

            if (visual == null)
                visual = GetComponent<EnemyVisual>();

            ResetRuntimeState();
        }

        protected virtual void OnEnable()
        {
            EnemyRegistry.Register(this);

            if (movement != null)
            {
                movement.ReachedGoal += ReachGoal;
                movement.DirectionChanged += HandleDirectionChanged;
            }
        }

        private void Start()
        {
            if (State != EnemyState.Spawning)
                return;

            BeginMovement();
        }

        protected virtual void OnDisable()
        {
            EnemyRegistry.Unregister(this);
            EternalTowers.Gameplay.Towers.PendingDamageRegistry.ReleaseAllForEnemy(this);

            if (movement != null)
            {
                movement.ReachedGoal -= ReachGoal;
                movement.DirectionChanged -= HandleDirectionChanged;
            }
        }

        public virtual void TakeDamage(float damage)
        {
            if (
                State == EnemyState.Dead ||
                State == EnemyState.ReachedGoal ||
                damage <= 0f
            )
                return;

            CurrentHealth = Mathf.Max(
                0f,
                CurrentHealth - damage * DamageTakenMultiplier
            );

            visual?.PlayHit();

            if (CurrentHealth <= 0f)
                Die();
        }

        public virtual void Die()
        {
            if (
                State == EnemyState.Dead ||
                State == EnemyState.ReachedGoal
            )
                return;

            State = EnemyState.Dead;
            EternalTowers.Gameplay.Towers.PendingDamageRegistry.ReleaseAllForEnemy(this);

            movement?.Stop();
            visual?.PlayDeath();

            Died?.Invoke();

            RewardGranted?.Invoke(reward);
            onRewardGranted?.Invoke(reward);

            if (!Application.isPlaying)
                return;

            if (deathAnimationDuration <= 0f)
                Destroy(gameObject);
            else
                StartCoroutine(DestroyAfterDeathAnimation());
        }

        public void ReachGoal()
        {
            if (
                State == EnemyState.Dead ||
                State == EnemyState.ReachedGoal
            )
                return;

            State = EnemyState.ReachedGoal;
            EternalTowers.Gameplay.Towers.PendingDamageRegistry.ReleaseAllForEnemy(this);

            movement?.Stop();

            BaseDamageRequested?.Invoke(baseDamage);
            onBaseDamageRequested?.Invoke(baseDamage);

            if (Application.isPlaying)
                Destroy(gameObject);
        }

        public void ResetRuntimeState()
        {
            CurrentHealth = maxHealth;
            State = EnemyState.Spawning;
        }

        public void SetMaxHealth(float value)
        {
            maxHealth = Mathf.Max(1f, value);
            CurrentHealth = maxHealth;
        }

        public void BeginMovement()
        {
            if (
                State == EnemyState.Dead ||
                State == EnemyState.ReachedGoal
            )
                return;

            if (path == null || path.WaypointCount <= 0)
                return;

            State = EnemyState.Moving;

            movement?.Begin(
                path,
                moveSpeed,
                laneOffset
            );
        }

        public void SetPath(EnemyPath enemyPath)
        {
            path = enemyPath;

            if (path == null)
                return;

            if (path.WaypointCount <= 0)
                return;

            if (State == EnemyState.Spawning)
                BeginMovement();
        }

        public void SetLaneOffset(float offset)
        {
            laneOffset = offset;

            if (
                path != null &&
                path.WaypointCount > 0 &&
                State == EnemyState.Spawning
            )
            {
                BeginMovement();
            }
        }

        public void SetDamageReductionSource(
            object source,
            float reductionPercent)
        {
            if (source == null)
                return;

            damageReductionSources[source] =
                Mathf.Clamp01(reductionPercent / 100f);
        }

        public void RemoveDamageReductionSource(object source)
        {
            if (source != null)
                damageReductionSources.Remove(source);
        }

        private float DamageTakenMultiplier
        {
            get
            {
                float totalReduction = 0f;

                foreach (
                    float reduction
                    in damageReductionSources.Values)
                {
                    totalReduction =
                        Mathf.Max(totalReduction, reduction);
                }

                return 1f - totalReduction;
            }
        }

        private void HandleDirectionChanged(Vector3 direction)
        {
            visual?.PlayMove(direction);
        }

        private IEnumerator DestroyAfterDeathAnimation()
        {
            yield return new WaitForSeconds(
                deathAnimationDuration
            );

            Destroy(gameObject);
        }
    }
}