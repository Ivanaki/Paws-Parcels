using System;
using System.Collections.Generic;
using Quests;
using R3;
using Services;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Scripts.Player
{
    //обязательно коллайдер на объекте или на дочернем(со скриптом redirector)
    //коллайдер должен быть всегда в центре
    public abstract class InteractableObject : DestinationObject
    {
        public abstract IReadOnlyList<string> ActivateLocations { get; }
        public abstract bool DontShowWithItemInHands { get; }
        public abstract ReadOnlyReactiveProperty<string> UIShowTextCode { get; }

        public override ReadOnlyReactiveProperty<bool> IsDestinationShowInUi => _isDestinationShow;

        public ReadOnlyReactiveProperty<bool> CanInteract => _canInteract;
        private readonly ReactiveProperty<bool> _canInteract = new(false);
        private readonly ReactiveProperty<bool> _highlighted = new(false);
        
        private readonly ReactiveProperty<bool> _isDestinationShow = new(false);

        [Inject] private PlayerService _playerService;
        
        private Vector3 _desPos;
        private float _currentNeedDistance;
            
        protected virtual void FixedUpdate()
        {
            if (IsQuestSelect.CurrentValue)
            {
                if (_playerService.PlayerTransform != null)
                {
                    _desPos = (_destinationShowUiPosition != null
                        ? _destinationShowUiPosition.position
                        : transform.position);
                    _currentNeedDistance = SelectedActivationDistance;

                    if (Vector3.Distance(_desPos, _playerService.PlayerTransform.position)
                        <= _currentNeedDistance)
                    {
                        _isDestinationShow.Value = true;
                        return;
                    }
                }

                _isDestinationShow.Value = false;
            }
            else
            {
                _isDestinationShow.Value = _highlighted.Value;
            }
        }
        

        protected void Start()
        {
            if (TryGetComponent(out SphereCollider colliderS))
                if (colliderS.center != Vector3.zero)
                    Debug.LogError("Collider center is not zero on interactable: " + gameObject.name);
            if (TryGetComponent(out BoxCollider colliderB))
                if (colliderB.center != Vector3.zero)
                    Debug.LogError("Collider center is not zero on interactable: " + gameObject.name);


            var collidersS = GetComponentsInChildren<SphereCollider>();
            foreach (var sphereCollider in collidersS)
                if (sphereCollider.center != Vector3.zero)
                    Debug.LogError("Collider center is not zero on interactable: " + gameObject.name + ", child:"
                                   + sphereCollider.gameObject.name);
            var collidersB = GetComponentsInChildren<BoxCollider>();
            foreach (var boxCollider in collidersB)
                if (boxCollider.center != Vector3.zero)
                    Debug.LogError("Collider center is not zero on interactable: " + gameObject.name + ", child:"
                                   + boxCollider.gameObject.name);
        }

        protected override void OnDisable()
        {
            _isDestinationShow.Value = false;
            base.OnDisable();
        }

        public abstract void Interact();

        public virtual void CanInteractWithMinDistance()
        {
            _canInteract.Value = true;
        }

        public virtual void CouldntInteractWithMinDistance()
        {
            _canInteract.Value = false;
        }
        
        public virtual void Highlight()
        {
            if(string.IsNullOrEmpty(UiShowTypeId) == false)
                //_canvasUiVisualizer.ShowOnCanvas(this);
                _highlighted.Value = true;
        }
        
        public virtual void Unhighlight()
        {
            if(string.IsNullOrEmpty(UiShowTypeId) == false)
                //_canvasUiVisualizer.HideOnCanvas(this);
                _highlighted.Value = false;
        }

        protected virtual void OnDestroy()
        {
            
        }
    }
}