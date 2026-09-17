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

        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Sprite Orientation")]
        [SerializeField] private bool useSpriteFlip = true;

        // Indica hacia qué lado mira el sprite ORIGINAL.
        // Si está activado: sprite original mira a la izquierda.
        // Si está desactivado: sprite original mira a la derecha.
        [SerializeField] private bool spriteFacesLeftByDefault = true;

        [Header("Animator States")]
        [SerializeField] private string idleState = "Idle";
        [SerializeField] private string moveState = "Walk";
        [SerializeField] private string hitState = "Hit";
        [SerializeField] private string deathState = "Death";

        private Direction currentDirection = Direction.Down;

        public bool HasAnimator => animator != null;

        private void Awake()
        {
            if (animator == null)
                animator = GetComponent<Animator>();

            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void PlayIdle()
        {
            TryPlayState(idleState);
        }

        public void PlayMove(Vector3 movement)
        {
            if (movement.sqrMagnitude <= 0.0001f)
                return;

            currentDirection = GetDominantDirection(movement);

            PlayState(moveState, currentDirection);
        }

        public void PlayHit()
        {
            // Conservamos la dirección actual.
            // El Hit no cambia hacia dónde mira el enemigo.
            TryPlayState(hitState);
        }

        public void PlayDeath()
        {
            // Death utiliza la última dirección conocida.
            PlayState(deathState, currentDirection);
        }

        private void PlayState(string stateName)
        {
            TryPlayState(stateName);
        }

        private void PlayState(
            string stateName,
            Direction direction)
        {
            if (
                animator == null ||
                string.IsNullOrWhiteSpace(stateName)
            )
            {
                return;
            }

            ApplyDirectionVisual(direction);

            if (HasParameter(animator, "Direction"))
            {
                animator.SetFloat(
                    "Direction",
                    (int)direction
                );
            }

            TryPlayState(stateName);
        }

        private void ApplyDirectionVisual(Direction direction)
        {
            if (
                spriteRenderer == null ||
                !useSpriteFlip
            )
            {
                return;
            }

            // Solo necesitamos invertir horizontalmente
            // cuando la dirección es izquierda o derecha.
            if (
                direction != Direction.Left &&
                direction != Direction.Right
            )
            {
                return;
            }

            bool movingRight =
                direction == Direction.Right;

            /*
             * Si el sprite original mira a la izquierda:
             *
             * Left  -> flip false
             * Right -> flip true
             *
             * Si el sprite original mira a la derecha:
             *
             * Left  -> flip true
             * Right -> flip false
             */
            if (spriteFacesLeftByDefault)
            {
                spriteRenderer.flipX = movingRight;
            }
            else
            {
                spriteRenderer.flipX = !movingRight;
            }
        }

        private void TryPlayState(string stateName)
        {
            if (
                animator == null ||
                string.IsNullOrWhiteSpace(stateName)
            )
            {
                return;
            }

            int stateHash =
                Animator.StringToHash(stateName);

            if (animator.HasState(0, stateHash))
            {
                PlayStateWithoutRestarting(stateName);
                return;
            }

            // Fallback para Walk.
            if (stateName == moveState)
            {
                string fallback = "Walk";

                int fallbackHash =
                    Animator.StringToHash(fallback);

                if (animator.HasState(0, fallbackHash))
                {
                    PlayStateWithoutRestarting(fallback);
                }
            }
        }

        private void PlayStateWithoutRestarting(
            string stateName)
        {
            int stateHash =
                Animator.StringToHash(stateName);

            AnimatorStateInfo currentState =
                animator.GetCurrentAnimatorStateInfo(0);

            if (currentState.shortNameHash != stateHash)
            {
                animator.Play(stateName);
            }
        }

        private static bool HasParameter(
            Animator animator,
            string parameterName)
        {
            if (
                animator == null ||
                string.IsNullOrWhiteSpace(parameterName)
            )
            {
                return false;
            }

            foreach (
                AnimatorControllerParameter parameter
                in animator.parameters)
            {
                if (parameter.name == parameterName)
                    return true;
            }

            return false;
        }

        private static Direction GetDominantDirection(
            Vector3 movement)
        {
            if (
                Mathf.Abs(movement.x) >
                Mathf.Abs(movement.y)
            )
            {
                return movement.x >= 0f
                    ? Direction.Right
                    : Direction.Left;
            }

            return movement.y >= 0f
                ? Direction.Up
                : Direction.Down;
        }
    }
}