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