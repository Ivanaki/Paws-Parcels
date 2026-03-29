using System.Collections.Generic;
using R3;
using UnityEngine;

namespace Paws_Parcels.Scripts.Player
{
    public abstract class BaseInteractable : InteractableObject
    {
        public override string DestinationId => _destinationId;
        public override string UiShowTypeId => _uiShowTypeId;
        public override ReadOnlyReactiveProperty<string> UIShowTextCode => _uiShowTextCodeRP;
        public override IReadOnlyList<string> ActivateLocations => _activeLocations;
        public override bool DontShowWithItemInHands => _dontShowWithItemInHands;
        
        protected override float SelectedActivationDistance => _selectedActivationDistance;
        
        private ReactiveProperty<string> _uiShowTextCodeRP = new("Bug");
        
        [Header("Base settings")]
        [SerializeField] private string _uiShowTypeId = "interact";
        [SerializeField] private string _uiShowTextCode;
        [SerializeField] private string _destinationId;
        [SerializeField] private List<string> _activeLocations = new(){"None"};
        [SerializeField] private bool _dontShowWithItemInHands = false;
        [SerializeField] private float _selectedActivationDistance = 20f;

        private void Awake()
        {
            _uiShowTextCodeRP.Value = _uiShowTextCode;
        }
    }
}