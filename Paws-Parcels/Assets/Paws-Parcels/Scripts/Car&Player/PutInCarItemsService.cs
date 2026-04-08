using System;
using System.Linq;
using Items;
using Messages;
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
        private readonly MessageSystem _messageSystem;
        private readonly GameProxy _gameProxy;

        private ReactiveProperty<PutItemState> _putState = new(PutItemState.Null);
        private CompositeDisposable _disposables = new();

        //private Transform _car = null;

        public PutInCarItemsService(GameStateProvider gameStateProvider, ItemsDatabase itemsDatabase,
            EditorDataService editorDataService, PlayerService playerService, ItemsSystem itemsSystem,
            MessageSystem messageSystem)
        {
            _itemsDatabase = itemsDatabase;
            _editorDataService = editorDataService;
            _itemsSystem = itemsSystem;
            _messageSystem = messageSystem;
            _gameProxy = gameStateProvider.GameProxy;
            
            _disposables.Add(messageSystem.AddListenerWithManyMessages("update_items_inside_message").Subscribe(_ =>
            {
                _gameProxy.ItemsInsideCar.ForEach(x =>
                {
                    _messageSystem.SendCompositeMessage(this, "put_inside_car:" + x.DestinationId);
                });
            }));
            
            _disposables.Add(_putState.Subscribe(state =>
            {
                if (state is PutItemState.Take or PutItemState.Put)
                {
                    playerService.StopMouseAndInteract(this);
                }
                else
                {
                    playerService.StartMouseAndInteract(this);
                }

                if (state is PutItemState.Null)
                {
                    _messageSystem.SendCompositeMessage(this, "throw_state_null");
                }

                if (state is PutItemState.Put)
                {
                    _messageSystem.SendCompositeMessage(this, "throw_state_put");
                }

                if (state is PutItemState.Take)
                {
                    _messageSystem.SendCompositeMessage(this, "throw_state_take");
                }
            }));
        }

        public bool TryCancel()
        {
            if (_putState.Value is PutItemState.Take or PutItemState.Put)
            {
                if (_putState.Value is PutItemState.Take)
                {
                    _messageSystem.SendCompositeMessage(this, "cancel_take_up");
                }
                else if (_putState.Value is PutItemState.Put)
                {
                    _messageSystem.SendCompositeMessage(this, "cancel_put_up");
                }
                
                
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
                    _messageSystem.SendCompositeMessage(this, "take_item_from_car_up:" + item.DestinationId);
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
                var itemDestinationId = _gameProxy.InHandsDestinationId;
                var result = _itemsSystem.TrySpecificDrop(position, yOffset);
                if (result)
                {
                    _isTimeToWait = true;
                    Observable.Timer(TimeSpan.FromSeconds(waitTime)).Subscribe(_ =>
                    {
                        _isTimeToWait = false;
                        _putState.Value = PutItemState.Null;
                        _messageSystem.SendCompositeMessage(this, "throw_up_end:" + itemDestinationId);
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
                    _gameProxy.InHandsDestinationId = lastItem.DestinationId;
                    
                    _messageSystem.SendCompositeMessage(this, "take_inside_car:" + lastItem.DestinationId);
                    
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
                    DestinationId = _gameProxy.InHandsDestinationId
                }));
                
                _messageSystem.SendCompositeMessage(this, "put_inside_car:" + _gameProxy.InHandsDestinationId);
                
                _gameProxy.IsItemInHandsNull.Value = true;
                _gameProxy.InHandsItemTypeId.Value = "";
                _gameProxy.InHandsDestinationId = "";
                
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