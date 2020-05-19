#pragma warning disable 0649
namespace Assets.MobileOptimizedWater.Scripts
{
    using UnityEngine;

    public class AnimationStarter : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Motion waterAnimation;

        public void Awake()
        {
            animator.Play(waterAnimation.name);
        }
    }
}
#pragma warning restore 0649