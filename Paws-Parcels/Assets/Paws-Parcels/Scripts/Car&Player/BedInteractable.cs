using System.Collections.Generic;
using Paws_Parcels.Scripts.Player;
using UnityEngine;

namespace Car_Player
{
    public class BedInteractable : BaseInteractable
    {
        public override IReadOnlyList<string> ActivateLocations => _activeLocations;
        [SerializeField] private List<string> _activeLocations = new(){"None"};
        
        public override void Interact()
        {
            
        }
    }
}