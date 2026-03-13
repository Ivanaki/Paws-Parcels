using System;
using System.Linq;
using Items;
using Paws_Parcels.Scripts.Player;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Services;
using UnityEngine;

namespace Car_Player
{
    public enum PutItemState
    {
        Null,
        Put,
        Take
    }
    
    public class PutInCarItemsService : IDisposable
    {
        public ReadOnlyReactiveProperty<PutItemState> PutState => _putState;
        
        private readonly ItemsDatabase _itemsDatabase;
        private readonly EditorDataService _editorDataService;
        private readonly ItemsSystem _itemsSystem;
        private readonly GameProxy _gameProxy;

        private ReactiveProperty<PutItemState> _putState = new(PutItemState.Null);
        private CompositeDisposable _disposables = new();

        //private Transform _car = null;

        public PutInCarItemsService(GameStateProvider gameStateProvider, ItemsDatabase itemsDatabase,
            EditorDataService editorDataService, PlayerService playerService, ItemsSystem itemsSystem)
        {
            _itemsDatabase = itemsDatabase;
            _editorDataService = editorDataService;
            _itemsSystem = itemsSystem;
            _gameProxy = gameStateProvider.GameProxy;
            
            _disposables.Add(_putState.Subscribe(state =>
            {
                if (state is PutItemState.Take or PutItemState.Put)
                {
                    playerService.StopMouseAndInteract(this);
                    //playerService.StopMovement(this);
                    
                    /*if(_car == null) FindCar();
                    
                    playerService.StartLookAt(_car, 1f);*/
                }
                else
                {
                    //playerService.StopLookAt();
                    
                    playerService.StartMouseAndInteract(this);
                    //playerService.StartMovement(this);
                }
            }));
        }

        /*private void FindCar()
        {
            _car = GameObject.FindGameObjectWithTag("Car").transform;
        }*/

        public bool TryCancel()
        {
            if (_putState.Value is PutItemState.Take or PutItemState.Put)
            {
                _putState.Value = PutItemState.Null;
                return true;
            }
            return false;
        }

        public void TakeItem(Item item)
        {
            if (_putState.Value is PutItemState.Take)
            {
                _putState.Value = PutItemState.Null;
                if (item.IsItemInCarUp)
                {
                    item.Interact();
                }
                else
                {
                    Debug.LogError("Item not in car up. Mistake");
                }
            }
        }

        private bool _isTimeToWait = false;
        
        public void PutPositionClicked(Vector3 position, float yOffset, float waitTime)
        {
            if(_isTimeToWait) return;
            
            if (_putState.Value is PutItemState.Put)
            {
                //заспавнить объект
                var result = _itemsSystem.TrySpecificDrop(position, yOffset);
                if (result)
                {
                    _isTimeToWait = true;
                    Observable.Timer(TimeSpan.FromSeconds(waitTime)).Subscribe(_ =>
                    {
                        _isTimeToWait = false;
                        _putState.Value = PutItemState.Null;
                    });
                }
                else
                {
                    _putState.Value = PutItemState.Null;
                }
            }
            else
            {
                Debug.LogError("Mistake");
            }
        }
        
        public bool TryPutOrTakeItemUpCar()
        {
            if (_putState.Value == PutItemState.Null)
            {
                if (_gameProxy.IsItemInHandsNull.Value)
                {
                    _putState.Value = PutItemState.Take;
                    return true;
                }
                else //что-то есть
                {
                    if (string.IsNullOrEmpty(_gameProxy.InHandsItemTypeId.Value))
                    {
                        Debug.LogError("InHandsItemTypeId is null or empty");
                        return false;
                    }
                    
                    _putState.Value = PutItemState.Put;
                    
                    return true;
                }
            }
            
            return false;
        }

        public bool TryPutOrTakeItemInsideCar()
        {
            if (_gameProxy.IsItemInHandsNull.Value)
            {
                if (_gameProxy.ItemsInsideCar.Count > 0)
                {
                    var lastItem = _gameProxy.ItemsInsideCar.Last();
                    _gameProxy.ItemsInsideCar.Remove(lastItem);
                    _gameProxy.IsItemInHandsNull.Value = false;
                    _gameProxy.InHandsItemTypeId.Value = lastItem.ItemTypeId;
                    _gameProxy.InHandsMarkId = lastItem.DestinationId;
                    return true;
                }
            }
            else //что-то есть
            {
                if (string.IsNullOrEmpty(_gameProxy.InHandsItemTypeId.Value))
                {
                    Debug.LogError("InHandsItemTypeId is null or empty");
                    return false;
                }
                
                if(_gameProxy.ItemsInsideCar.Count == _editorDataService.EditorSettings.MaxItemsInCar) return false;
                if (_itemsDatabase.ItemsTypeInWorldDict.TryGetValue(_gameProxy.InHandsItemTypeId.Value, out var item))
                {
                    if(item.ItemLargeType != ItemLargeType.Small) return false;
                }
                else
                {
                    Debug.LogError("Mistake");
                    return false;
                }
                
                _gameProxy.ItemsInsideCar.Add(new(new ()
                {
                    ItemTypeId = _gameProxy.InHandsItemTypeId.Value,
                    DestinationId = _gameProxy.InHandsMarkId
                }));
                
                _gameProxy.IsItemInHandsNull.Value = true;
                _gameProxy.InHandsItemTypeId.Value = "";
                _gameProxy.InHandsMarkId = "";
                
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            _putState?.Dispose();
            _disposables?.Dispose();
        }
    }
}