using System;
using System.Collections.Generic;
using Items;
using ObservableCollections;
using Paws_Parcels.Scripts;
using R3;
using Services;
using UnityEngine;
using Zenject;

namespace Car_Player
{
    public class ItemsInsideVisualizer : BaseVisualizer
    {
        /// <summary>
        /// count = 6
        /// </summary>
        [SerializeField] private List<Transform> _itemPositions;
        private List<GameObject> _itemsCurrentActive = new();
        
        [Inject] private ItemsDatabase _itemsDatabase;
        [Inject] private EditorDataService _editorDataService;


        private void Awake()
        {
            _itemPositions.ForEach(x =>
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

            UpdateItems();
            
            _disposable.Add(_gameProxy.ItemsInsideCar.ObserveChanged().Subscribe(e =>
                UpdateItems()));
        }

        private void UpdateItems()
        {
            if (_gameProxy.ItemsInsideCar.Count > _editorDataService.EditorSettings.MaxItemsInCar)
            {
                Debug.LogError("Too many items inside car");
                return;
            }
            
            _itemsCurrentActive.ForEach(Destroy);
            _itemsCurrentActive.Clear();
            
            for (var i = 0; i < _gameProxy.ItemsInsideCar.Count; i++)
            {
                var item = _gameProxy.ItemsInsideCar[i];

                if (_itemsDatabase.ItemsTypeInsideCarPrefabsDict.TryGetValue(item.ItemTypeId, out var itemPrefab))
                {
                    var spawned = Instantiate(itemPrefab, _itemPositions[i]);
                    spawned.transform.localPosition = itemPrefab.LocalPosition;
                    spawned.transform.localRotation = itemPrefab.LocalRotation;
                    _itemsCurrentActive.Add(spawned.gameObject);
                }
            }
        }
    }
}