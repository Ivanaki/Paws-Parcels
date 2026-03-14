using System.Collections.Generic;
using Paws_Parcels.Scripts.Player;
using UnityEngine;
using Zenject;

namespace Car_Player
{
    public class PutUpInteractable : BaseInteractable
    {
        [Inject] private PutInCarItemsService _putInCarItemsService;
        
        public override void Interact()
        {
            _putInCarItemsService.TryPutOrTakeItemUpCar();
        }
    }
}