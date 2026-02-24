using Car_Player;
using Zenject;

namespace Paws_Parcels.Scripts.Player
{
    public class Door : InteractableObject
    {
        [Inject] private DoorService _doorService;
        
        public override void Interact()
        {
            _doorService.ChangeDoor();
        }
    }
}