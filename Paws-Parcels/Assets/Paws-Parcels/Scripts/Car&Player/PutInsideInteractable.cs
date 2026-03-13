using System.Collections.Generic;
using Paws_Parcels.Scripts.Player;
using UnityEngine;
using Zenject;

namespace Car_Player
{
    public class PutInsideInteractable : BaseInteractable
    {
        public override IReadOnlyList<string> ActivateLocations => _activeLocations;
        [SerializeField] private List<string> _activeLocations = new(){"None"};
        
        [Inject] private PutInCarItemsService _putInCarItemsService;
        
        public override void Interact()
        {
            _putInCarItemsService.TryPutOrTakeItemInsideCar();
        }
    }
}