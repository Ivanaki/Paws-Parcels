using System.Collections.Generic;
using UnityEngine;

namespace Paws_Parcels.Scripts.Player
{
    public abstract class BaseInteractable : InteractableObject
    {
        public override string DestinationId => _destinationId;
        public override string UiShowTypeId => _uiShowTypeId;
        public override string UIShowTextCode => _uiShowTextCode;
        public override IReadOnlyList<string> ActivateLocations => _activeLocations;
        public override bool DontShowWithItemInHands => _dontShowWithItemInHands;
        
        [Header("Base settings")]
        [SerializeField] private string _uiShowTypeId = "interact";
        [SerializeField] private string _uiShowTextCode;
        [SerializeField] private string _destinationId;
        [SerializeField] private List<string> _activeLocations = new(){"None"};
        [SerializeField] private bool _dontShowWithItemInHands = false;
    }
}