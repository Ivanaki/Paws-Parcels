using System;
using Items;
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
        public string ItemTypeId => _itemTypeId;
        public override string DestinationId => _destinationId;
        public override string UiShowTypeId => _uiShowTypeId;
        public override string UiShowText => _uiShowText;

        [Header("Item prefab settings")]
        [SerializeField] private string _itemTypeId;
        [SerializeField] private string _uiShowTypeId;
        [SerializeField] private string _uiShowText;
        
        [Header("Item in scene changes")]
        [SerializeField] private string _destinationId;
        
        private ItemsInWorldVisualizer _itemsInWorldVisualizer;
        private ItemProxy _itemProxy;
        
        private bool _isInstantiated = false;
        private Rigidbody _rigidbody = null;

        private IDisposable _disposable;
        
        public void ConstructItem(ApplyInFixUpdate applyInFixUpdate, ItemProxy itemProxy, ItemsInWorldVisualizer itemsInWorldVisualizer)
        {
            _isInstantiated = true;
            InitializeRigidbody();
            
            _itemProxy = itemProxy;
            _itemsInWorldVisualizer = itemsInWorldVisualizer;
            
            _destinationId = itemProxy.DestinationId;
            
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
            
            InitializeRigidbody();
        }

        private void InitializeRigidbody()
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
        
        public Vector3 GetLinearVelocity() => _rigidbody.linearVelocity;
        public Vector3 GetAngularVelocity() => _rigidbody.angularVelocity;
    }
}