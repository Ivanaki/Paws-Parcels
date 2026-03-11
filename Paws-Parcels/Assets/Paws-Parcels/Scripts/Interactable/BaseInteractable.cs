using UnityEngine;

namespace Paws_Parcels.Scripts.Player
{
    public abstract class BaseInteractable : InteractableObject
    {
        public override string DestinationId => _destinationId;
        public override string UiShowTypeId => _uiShowTypeId;
        public override string UIShowTextCode => _uiShowTextCode;
        
        [Header("Door settings")]
        [SerializeField] private string _uiShowTypeId = "interact";
        [SerializeField] private string _uiShowTextCode;
        [SerializeField] private string _destinationId;
    }
}