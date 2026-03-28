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
        
        public ReadOnlyReactiveProperty<bool> CanInteract => _canInteract;
        private readonly ReactiveProperty<bool> _canInteract = new(false);

        [Inject] protected CanvasUiVisualizer _canvasUiVisualizer;

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
                _canvasUiVisualizer.ShowOnCanvas(this);
        }
        
        public virtual void Unhighlight()
        {
            if(string.IsNullOrEmpty(UiShowTypeId) == false)
                _canvasUiVisualizer.HideOnCanvas(this);
        }
    }
}