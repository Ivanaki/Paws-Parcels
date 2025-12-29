using StarterAssets;
using UnityEngine;

namespace MotorHome.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class StepsAnimator : MonoBehaviour
    {
        [SerializeField] private ThirdPersonController _thirdPersonController;
        
        public void OnFootstep(AnimationEvent animationEvent)
        {
            _thirdPersonController.OnFootstep(animationEvent);
        }
    }
}