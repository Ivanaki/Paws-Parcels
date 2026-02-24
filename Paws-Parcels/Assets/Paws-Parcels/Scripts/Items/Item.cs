using System;
using Items;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using UnityEngine;

namespace Paws_Parcels.Scripts.Player
{
    /// <summary>
    /// Обязательно, если это префаб, то отключить его active
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Item : InteractableObject
    {
        public Vector3 LinearVelocity => _rigidbody.linearVelocity;
        public Vector3 AngularVelocity => _rigidbody.angularVelocity;
        public string ItemTypeId => _itemTypeId;
        
        [Header("Item Settings")]
        [SerializeField] private string _itemTypeId;
        
        private ItemsInWorldVisualizer _itemsInWorldVisualizer;
        private ItemProxy _itemProxy;
        
        private bool _isInstantiated = false;
        private Rigidbody _rigidbody = null;

        private IDisposable _disposable;
        
        public void ConstructItem(ApplyInFixUpdate applyInFixUpdate, ItemProxy itemProxy, ItemsInWorldVisualizer itemsInWorldVisualizer)
        {
            _isInstantiated = true;
            UpdateRigidbody();
            
            _itemProxy = itemProxy;
            _itemsInWorldVisualizer = itemsInWorldVisualizer;
            
            SetMarkId(itemProxy.MarkId);
            
            transform.position = itemProxy.Position;
            transform.rotation = itemProxy.Rotation;
            
            _disposable = applyInFixUpdate.ObserveApply().Take(1).Subscribe(_ =>
            {
                if (gameObject == null) return;
                
                transform.position = itemProxy.Position;
                transform.rotation = itemProxy.Rotation;
                _rigidbody.linearVelocity = itemProxy.LinearVelocity;
                _rigidbody.angularVelocity = itemProxy.AngularVelocity;
            });
        }
        
        private void Awake()
        {
            //если объект находится на сцене(не инициализировался) сразу, то убираем его
            if (_isInstantiated == false)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
                return;
            }
            
            UpdateRigidbody();
        }

        private void UpdateRigidbody()
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }
        }
        
        public override void Interact()
        {
            _itemsInWorldVisualizer.RequestPickUp(_itemProxy);
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}