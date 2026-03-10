using UnityEngine;

namespace Paws_Parcels.Scripts.Player
{
    public abstract class BaseInteractable : InteractableObject
    {
        public override string DestinationId => _destinationId;
        public override string UiShowTypeId => _uiShowTypeId;
        public override string UiShowText => _uiShowText;
        
        [Header("Door settings")]
        [SerializeField] private string _uiShowTypeId = "interact";
        [SerializeField] private string _uiShowText;
        [SerializeField] private string _destinationId;
    }
}