using UnityEngine;

namespace EternalTowers.Gameplay.Enemies
{
    public class EnemyVisual : MonoBehaviour
    {
        private enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        [SerializeField] private Animator animator;
        [SerializeField] private string idleState = "Idle";
        [SerializeField] private string moveState = "Move";
        [SerializeField] private string hitState = "Hit";
        [SerializeField] private string deathState = "Death";

        public bool HasAnimator => animator != null;

        public void PlayIdle()
        {
            PlayState(idleState);
        }

        public void PlayMove(Vector3 movement)
        {
            Direction direction = GetDominantDirection(movement);
            PlayState(moveState, direction);
        }

        public void PlayHit()
        {
            PlayState(hitState);
        }

        public void PlayDeath()
        {
            PlayState(deathState);
        }

        private void PlayState(string stateName)
        {
            if (animator != null && !string.IsNullOrWhiteSpace(stateName))
                animator.Play(stateName);
        }

        private void PlayState(string stateName, Direction direction)
        {
            if (animator == null || string.IsNullOrWhiteSpace(stateName))
                return;

            animator.SetInteger("Direction", (int)direction);
            animator.Play(stateName);
        }

        private static Direction GetDominantDirection(Vector3 movement)
        {
            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
                return movement.x >= 0f ? Direction.Right : Direction.Left;

            return movement.y >= 0f ? Direction.Up : Direction.Down;
        }
    }
}