using UnityEngine;

namespace EternalTowers.Gameplay.Towers
{
    public class TargetingRuntime : MonoBehaviour
    {
        [SerializeField] private TargetPriority targetPriority = TargetPriority.First;

        public TargetPriority Priority
        {
            get => targetPriority;
            set => targetPriority = value;
        }
    }
}
