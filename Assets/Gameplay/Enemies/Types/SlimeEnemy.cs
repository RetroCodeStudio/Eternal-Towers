using UnityEngine;

namespace EternalTowers.Gameplay.Enemies
{
    public class SlimeEnemy : Enemy
    {
        [Header("Split")]
        [SerializeField] private SlimeEnemy smallerSlimePrefab;
        [SerializeField, Min(1)] private int splitCount = 2;
        [SerializeField, Min(0.01f)] private float childHealthMultiplier = 0.5f;
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
                SlimeEnemy child = Instantiate(
                    smallerSlimePrefab,
                    transform.position,
                    transform.rotation);

                child.SetMaxHealth(MaxHealth * childHealthMultiplier);
                child.SetCanSplit(false);

                CopyNavigationTo(child);
                NotifySplitChildSpawned(child);
            }
        }
    }
}