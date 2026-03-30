using System;
using System.Collections.Generic;
using Items;
using ObservableCollections;
using Paws_Parcels.Scripts.Player;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using UnityEngine;
using Zenject;

namespace Destinations
{
    public class ItemDestinationPutter : InteractableObject
    {
        public override string DestinationId => _destinationId;
        public override string UiShowTypeId => _uiShowTypeId;
        public override string MapShowTypeId => _mapShowTypeId;
        public override ReadOnlyReactiveProperty<string> UIShowTextCode => _uiShowTextCodeRP;
        public override IReadOnlyList<string> ActivateLocations => _activeLocations;
        public override bool DontShowWithItemInHands => _dontShowWithItemInHands;
        protected override float SelectedActivationDistance => _selectedActivationDistance;
        
        [Header("Interactable Settings")] 
        [SerializeField] private string _uiShowTypeId = "interact";
        [SerializeField] private string _mapShowTypeId = "dot";
        [SerializeField] private string _uiShowTextCodeWithItem;
        [SerializeField] private string _uiShowTextCodeWithoutItem;
        [SerializeField] private string _destinationId;
        [SerializeField] private List<string> _activeLocations = new(){"None"};
        [SerializeField] private bool _dontShowWithItemInHands = false;
        [SerializeField] private float _selectedActivationDistance = 20f;
        
        [Header("ItemDestination Settings")]
        [SerializeField] private string _itemDestinationPutterId;
        [SerializeField] private List<Transform> _itemsPositions;

        [Inject] private GameProxy _gameProxy;
        [Inject] private ItemDestinationPutterService _itemDestinationPutterService;
        [Inject] private ItemsDatabase _itemsDatabase;
        
        private List<GameObject> _itemsCurrentActive = new();
        private IDisposable _subscription;
        
        private ReactiveProperty<string> _uiShowTextCodeRP = new("Bug");

        private void Awake()
        {
            _uiShowTextCodeRP.Value = _uiShowTextCodeWithoutItem;
            
            _itemsPositions.ForEach(x =>
            {
                foreach (Transform child in x)
                {
                    Destroy(child.gameObject);
                }
            });
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            UpdateSubscription();
            UpdateItems();
            
            _disposables.Add(_gameProxy.IsItemInHandsNull.Subscribe(isNull => 
                _uiShowTextCodeRP.Value = isNull ? _uiShowTextCodeWithoutItem : _uiShowTextCodeWithItem));
            
            _disposables.Add(_gameProxy.ItemsDestinationsPutters.ObserveChanged().Subscribe(e =>
            {
                UpdateSubscription();
                UpdateItems();
            }));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _subscription?.Dispose();
        }

        public override void Interact()
        {
            _itemDestinationPutterService.TryPutOrTakeItemInDestination(_itemDestinationPutterId,
                _itemsPositions.Count);
        }

        private void UpdateSubscription()
        {
            _subscription?.Dispose();
            if (_gameProxy.ItemsDestinationsPutters.TryGetValue(_itemDestinationPutterId, out var itemDestinationProxy))
            {
                _subscription = itemDestinationProxy.Items.ObserveChanged().Subscribe(_ => UpdateItems());
            }
        }
        
        private void UpdateItems()
        {
            _itemsCurrentActive.ForEach(Destroy);
            _itemsCurrentActive.Clear();
            
            if (_gameProxy.ItemsDestinationsPutters.TryGetValue(_itemDestinationPutterId,
                    out var itemDestinationsPutter))
            {
                if (itemDestinationsPutter.Items.Count > _itemsPositions.Count)
                {
                    throw new Exception("itemDestinationsPutter items count > max _itemsPositions");
                }
                
                for (var i = 0; i < itemDestinationsPutter.Items.Count; i++)
                {
                    var item = itemDestinationsPutter.Items[i];

                    if (_itemsDatabase.ItemsTypeInsideCarPrefabsDict.TryGetValue(item.ItemTypeId, out var itemPrefab))
                    {
                        var spawned = Instantiate(itemPrefab, _itemsPositions[i]);
                        spawned.transform.localPosition = itemPrefab.LocalPosition;
                        spawned.transform.localRotation = itemPrefab.LocalRotation;
                        _itemsCurrentActive.Add(spawned.gameObject);
                    }
                }
            }
        }
    }
}