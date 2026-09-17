using UnityEngine;

namespace EternalTowers.Gameplay.Enemies
{
    public class SlimeEnemy : Enemy
    {
        [Header("Split")]
        [SerializeField] private SlimeEnemy smallerSlimePrefab;
        [SerializeField, Min(1)] private int splitCount = 2;
        [SerializeField, Min(0.01f)] private float childHealthMultiplier = 0.5f;
        [SerializeField, Min(0f)] private float splitRadius = 0.35f;
        [SerializeField] private bool canSplit = true;

        private bool hasSplit;

        public override void Die()
        {
            if (State == EnemyState.Dead || State == EnemyState.ReachedGoal)
                return;

            SplitOnce();
            base.Die();
        }

        public void SetCanSplit(bool value)
        {
            canSplit = value;
        }

        private void SplitOnce()
        {
            if (!canSplit || hasSplit || smallerSlimePrefab == null || !Application.isPlaying)
                return;

            hasSplit = true;

            for (int index = 0; index < splitCount; index++)
            {
                Vector2 offset = Random.insideUnitCircle * splitRadius;
                SlimeEnemy child = Instantiate(
                    smallerSlimePrefab,
                    transform.position + new Vector3(offset.x, offset.y, 0f),
                    transform.rotation);

                child.SetMaxHealth(MaxHealth * childHealthMultiplier);
                child.SetCanSplit(false);

                if (Path != null)
                    child.SetPath(Path);

                child.BeginMovement();
            }
        }
    }
}