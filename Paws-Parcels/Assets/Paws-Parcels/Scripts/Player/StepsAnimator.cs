
using UnityEngine;

namespace Paws_Parcels.Scripts.Player
{
    [RequireComponent(typeof(Animator))]
    public class StepsAnimator : MonoBehaviour
    {
        [SerializeField] private ThirdPersonController _thirdPersonController;
        [SerializeField] private Transform _leftFoot;
        [SerializeField] private Transform _rightFoot;
        
        public void OnFootstep(AnimationEvent animationEvent)
        {
            _thirdPersonController.OnFootstep(animationEvent, (_leftFoot.position + _rightFoot.position)/2);
        }

        public void OnLand(AnimationEvent animationEvent)
        {
            _thirdPersonController.OnLand(animationEvent);
        }
    }
}