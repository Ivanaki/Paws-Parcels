using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts.Saves
{
    public class ItemsSaveManager : MonoBehaviour
    {
        [SerializeField] private CarInventory _carInventory;
        [SerializeField] private Item[] _items;
        
        private CompositeDisposable _disposable;
        private GameStateProvider _gameStateProvider;
        private PlayerPickUpper _playerPickUpper;
        //private ApplyInFixUpdate _applyInFixUpdate;
        
        [Inject]
        private void Construct(GameStateProvider gameStateProvider, PlayerPickUpper playerPickUpper/*, ApplyInFixUpdate applyInFixUpdate*/)
        {
            _gameStateProvider =  gameStateProvider;
            _playerPickUpper = playerPickUpper;
            //_applyInFixUpdate = applyInFixUpdate;
        }

        
        private void OnEnable()
        {
            _disposable = new CompositeDisposable();
            IDisposable dis;

            dis = _gameStateProvider.Updated.Subscribe(_ =>
            {
                if(_gameStateProvider.CoreState.Items == null) Debug.LogError($"{nameof(_gameStateProvider.CoreState.Items)} is null");
                //if(_gameStateProvider.CoreState.Items) Debug.LogError($"{nameof(_gameStateProvider.CoreState.Items)} is null");
                ItemData[] itemDatas = _gameStateProvider.CoreState.Items;
                Item itemToHands = null;
                List<(Item, int)> itemsToCarInside = new();
                List<(Item, int)> itemsToCarUp = new();
                
                
                for (var i = 0; i < itemDatas.Length; i++)
                {
                    var itemData = itemDatas[i];
                    var item = _items[i];

                    
                    
                    if (itemData.Type == Item.ItemPlace.Free)
                    {
                        item.transform.SetParent(null);
                        item.SetObjectTouchable();
                        /*_applyInFixUpdate.ObserveApply().Take(1).Subscribe(_ =>
                        {
                            item.transform.position = itemData.Position.ToUnity();
                            item.transform.rotation = itemData.Rotation.ToUnity();
                            item.Rigidbody.linearVelocity = itemData.LinearVelocity.ToUnity();
                            item.Rigidbody.angularVelocity = itemData.AngularVelocity.ToUnity();
                        });*/
                    }
                    else if (itemData.Type == Item.ItemPlace.Hands)
                    {
                        itemToHands = item;
                    }
                    else if(itemData.Type == Item.ItemPlace.CarInside)
                    {
                        itemsToCarInside.Add((item, itemData.QueueNumberInsideCar));
                    }
                    else if (itemData.Type == Item.ItemPlace.CarUp)
                    { 
                        itemsToCarUp.Add((item, itemData.QueueNumberUpCar));
                    }
                    else
                    {
                        Debug.LogError($"Unknown item type: {itemData.Type}");
                    }
                }
                
                itemsToCarInside = itemsToCarInside.OrderBy(x => x.Item2).ToList();
                var itemsToCarInsideList = itemsToCarInside.Select(x => x.Item1).ToArray();
                
                itemsToCarUp = itemsToCarUp.OrderBy(x => x.Item2).ToList();
                var itemsToCarUpList = itemsToCarUp.Select(x => x.Item1).ToArray();
                
                _playerPickUpper.SetSavedItem(itemToHands);
                _carInventory.SetSavedItemsInside(itemsToCarInsideList);
                _carInventory.SetSavedItemsUp(itemsToCarUpList);
            }); _disposable.Add(dis);

            dis = _gameStateProvider.Saved.Subscribe(_ =>
            {
                ItemData[] itemDatas = new ItemData[_items.Length];
                for (var i = 0; i < _items.Length; i++)
                {
                    itemDatas[i] = new ItemData(_items[i], _carInventory.GetItemNumberInside(_items[i]),
                        _carInventory.GetItemNumberUp(_items[i]));
                }
                _gameStateProvider.CoreState.Items =  itemDatas;
            });_disposable.Add(dis);
            
        }
        
        private void OnDisable()
        {
            _disposable?.Dispose();
        }
    }
}