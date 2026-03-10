using Paws_Parcels.Scripts.Player;
using Zenject;

namespace Car_Player
{
    public class PutInsideInteractable : BaseInteractable
    {
        [Inject] private PutInCarItemsService _putInCarItemsService;
        
        public override void Interact()
        {
            _putInCarItemsService.TryPutOrTakeItemInsideCar();
        }
    }
}