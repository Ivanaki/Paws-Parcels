using Paws_Parcels.Scripts.Car_Player;
using Paws_Parcels.Scripts.Player;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Car
{
    public class EnterDoor : BaseInteractable
    {
        private static readonly int Sit = Animator.StringToHash("Sit");
        [SerializeField] private Animator _doorAnimator;
        
        [Inject] private SitInCarService _sitInCarService;
        
        public override void Interact()
        {
            var result = _sitInCarService.TrySit();
            if (result)
            {
                _doorAnimator.SetTrigger(Sit);
            }
        }
    }
}