using Paws_Parcels.Scripts.Car_Player;
using Paws_Parcels.Scripts.Player;
using Zenject;

namespace Paws_Parcels.Car
{
    public class EnterDoor : InteractableObject
    {
        [Inject] private SitInCarService _sitInCarService;
        
        public override void Interact()
        {
            _sitInCarService.TrySit();
        }
    }
}