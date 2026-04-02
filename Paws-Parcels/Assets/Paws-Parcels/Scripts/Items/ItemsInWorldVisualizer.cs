using System;
using System.Collections.Generic;
using ObservableCollections;
using Paws_Parcels.Scripts.Player;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using UnityEngine;
using Zenject;

namespace Items
{
    public class ItemsInWorldVisualizer : IInitializable, IDisposable
    {
        private readonly ApplyInFixUpdate _applyInFixUpdate;
        private readonly ItemsSystem _itemsSystem;
        private readonly DiContainer _container;
        private readonly ObservableList<ItemProxy> _itemsInWorld;
        private readonly IReadOnlyDictionary<string, Item> _itemsTypeDict;
        private readonly CompositeDisposable _disposable = new();
        private readonly Observable<Unit> _collectDataBeforeSave;
        
        private readonly Dictionary<ItemProxy, Item> _itemsDict = new();
        
        public ItemsInWorldVisualizer(GameStateProvider gameStateProvider, ItemsDatabase itemsDatabase, 
            ApplyInFixUpdate applyInFixUpdate, ItemsSystem itemsSystem, DiContainer container)
        {
            _applyInFixUpdate = applyInFixUpdate;
            _itemsSystem = itemsSystem;
            _container = container;
            _itemsInWorld = gameStateProvider.GameProxy.ItemsInWorld;
            _itemsTypeDict = itemsDatabase.ItemsTypeInWorldDict;
            _collectDataBeforeSave = gameStateProvider.CollectDataBeforeSave;
        }
        
        public void Initialize()
        {
            _itemsInWorld.ForEach(SpawnItem);
            
            _disposable.Add(_itemsInWorld.ObserveAdd().Subscribe(e =>
                SpawnItem(e.Value)));
            
            _disposable.Add(_itemsInWorld.ObserveRemove().Subscribe(e =>
                DeleteItem(e.Value)));
            
            _disposable.Add(_itemsInWorld.ObserveClear().Subscribe(e =>
                DeleteAllItems()));

            _disposable.Add(_collectDataBeforeSave.Subscribe(_ =>
            {
                _itemsInWorld.ForEach(x =>
                {
                    var item = _itemsDict[x];
                    x.Position = item.transform.position;
                    x.Rotation = item.transform.rotation;
                    x.LinearVelocity = item.GetLinearVelocity();
                    x.AngularVelocity = item.GetAngularVelocity();
                });
            }));
        }

        private void SpawnItem(ItemProxy itemProxy)
        {
            if (_itemsTypeDict.ContainsKey(itemProxy.ItemTypeId) == false)
            {
                Debug.Log("Смотри ошибку выше в сообщениях");
                return;
            }
            
            var spawned = _container.InstantiatePrefabForComponent<Item>(_itemsTypeDict[itemProxy.ItemTypeId]);
            
            spawned.ConstructItem(_applyInFixUpdate, itemProxy, this);
            spawned.gameObject.SetActive(true);
            _itemsDict[itemProxy] = spawned;
        }

        private void DeleteItem(ItemProxy itemProxy)
        {
            var item = _itemsDict[itemProxy];
            item.gameObject.SetActive(false);
            UnityEngine.Object.Destroy(item.gameObject);
            _itemsDict.Remove(itemProxy);
        }

        private void DeleteAllItems()
        {
            foreach (var item in _itemsDict.Values)
            {
                item.gameObject.SetActive(false);
                UnityEngine.Object.Destroy(item.gameObject);
            }
            
            _itemsDict.Clear();
        }

        public void RequestPickUp(ItemProxy itemProxy)
        {
            _itemsSystem.TryPickUp(itemProxy);
        }
        
        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}