using System;
using System.Collections.Generic;
using Car_Player;
using Items;
using R3;
using Services;
using UnityEngine;

namespace Paws_Parcels.Scripts.Player
{
    public enum ItemLargeType
    {
        Big,
        Small
    }
    
    /// <summary>
    /// Обязательно, если это префаб, то отключить его active
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Item : InteractableObject
    {
        private class ShowOnCanvasCircle : IShowableInUI
        {
            private readonly Transform _transformParent;

            public ShowOnCanvasCircle(Transform transformParent)
            {
                _transformParent = transformParent;
            }
            public string UiShowTypeId => "circle";
            public Vector3 GetGlobalPositionShowablePoint() => _transformParent.position;
            public bool IsDestroyed() => _transformParent.gameObject == null;
        }
        
        public bool IsItemInCarUp => _isItemInCarUp;
        public string ItemTypeId => _itemTypeId;
        public ItemLargeType ItemLargeType => _itemLargeType;
        public override string DestinationId => _destinationId;
        public override string UiShowTypeId => _uiShowTypeId;
        public override string MapShowTypeId => _mapShowTypeId;
        public override ReadOnlyReactiveProperty<string> UIShowTextCode => _uiShowTextCodeRP;
        public override bool DontShowWithItemInHands => _dontShowWithItemInHands;
        protected override float SelectedActivationDistance => _selectedActivationDistance;

        public override IReadOnlyList<string> ActivateLocations => _activeLocations;

        public Observable<Unit> OnDestroyObserve => _onDestroySubject;

        [Header("Item prefab settings")]
        [SerializeField] private string _itemTypeId;
        [SerializeField] private string _uiShowTypeId = "interact";
        [SerializeField] private string _mapShowTypeId = "quest";
        [SerializeField] private string _uiShowTextCode;
        [SerializeField] private ItemLargeType _itemLargeType;
        [SerializeField] private List<string> _activeLocations = new(){"None"};
        [SerializeField] private bool _dontShowWithItemInHands = true;
        [SerializeField] private float _selectedActivationDistance = 20f;
        
        [Header("Item in scene changes")]
        [SerializeField] private string _destinationId;
        
        private readonly Subject<Unit> _onDestroySubject = new();
        
        private ReactiveProperty<string> _uiShowTextCodeRP = new("Bug");
        
        private ItemsInWorldVisualizer _itemsInWorldVisualizer;
        private ItemProxy _itemProxy;
        
        private bool _isInstantiated = false;
        private Rigidbody _rigidbody = null;

        private bool _isItemInCarUp = false;
        private HashSet<object> _highlightInitiators = new();
        private ShowOnCanvasCircle _showOnCanvasCircle;
        private CarUpService _carUpService;

        private IDisposable _disposable;
        
        public void ConstructItem(ApplyInFixUpdate applyInFixUpdate, ItemProxy itemProxy, 
            ItemsInWorldVisualizer itemsInWorldVisualizer, CarUpService carUpService)
        {
            _uiShowTextCodeRP.Value = _uiShowTextCode;
            
            _isInstantiated = true;
            InitializeRigidbody();
            
            _itemProxy = itemProxy;
            _itemsInWorldVisualizer = itemsInWorldVisualizer;
            _carUpService =  carUpService;
            
            _destinationId = itemProxy.DestinationId;
            
            transform.position = itemProxy.Position;
            transform.rotation = itemProxy.Rotation;
            
            _disposable = applyInFixUpdate.ObserveApply().Take(1).Subscribe(_ =>
            {
                if (gameObject == null) return;
                
                transform.position = itemProxy.Position;
                transform.rotation = itemProxy.Rotation;
                _rigidbody.WakeUp();
                _rigidbody.linearVelocity = itemProxy.LinearVelocity;
                _rigidbody.angularVelocity = itemProxy.AngularVelocity;
            });
            
            
        }
        
        
        private void Awake()
        {
            _showOnCanvasCircle = new ShowOnCanvasCircle(transform);
            
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

        public void HighlightItem(object scriptToHighlight)
        {
            _highlightInitiators.Add(scriptToHighlight);
            UpdateHighlight();
        }

        public void UnhighlightItem(object scriptToUnhighlight)
        {
            _highlightInitiators.Remove(scriptToUnhighlight);
            UpdateHighlight();
        }
        
        private void UpdateHighlight()
        {
            if (_highlightInitiators.Count > 0)
            {
                _canvasUiVisualizer.ShowOnCanvas(_showOnCanvasCircle);
            }
            else
            {
                _canvasUiVisualizer.HideOnCanvas(_showOnCanvasCircle);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _disposable?.Dispose();
            _onDestroySubject.OnNext(Unit.Default);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("CarUp"))
            {
                _isItemInCarUp = true;
                _carUpService.AddItem(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("CarUp"))
            {
                _isItemInCarUp = false;
                _carUpService.RemoveItem(this);
            }
        }

        public Vector3 GetLinearVelocity() => _rigidbody.linearVelocity;
        public Vector3 GetAngularVelocity() => _rigidbody.angularVelocity;
    }
}