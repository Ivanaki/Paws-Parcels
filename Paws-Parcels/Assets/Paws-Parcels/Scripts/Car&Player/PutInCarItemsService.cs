using System.Linq;
using Items;
using Paws_Parcels.Scripts.Player;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Unity.VisualScripting;
using UnityEngine;

namespace Car_Player
{
    public class PutInCarItemsService
    {
        private readonly ItemsDatabase _itemsDatabase;
        private readonly GameProxy _gameProxy;

        public PutInCarItemsService(GameStateProvider gameStateProvider, ItemsDatabase itemsDatabase)
        {
            _itemsDatabase = itemsDatabase;
            _gameProxy = gameStateProvider.GameProxy;
        }

        public bool TryPutOrTakeItemUpCar()
        {
            /*if (_gameProxy.IsItemInHandsNull.Value)
            {
                
            }
            else //что-то есть
            {
                if (string.IsNullOrEmpty(_gameProxy.InHandsItemTypeId.Value)) return false;
                
                _gameProxy.ItemsUpCar.Add(new ItemUpCarProxy(new ItemUpCar()
                {
                    ItemTypeId = _gameProxy.InHandsItemTypeId.Value,
                    DestinationId = _gameProxy.InHandsMarkId,
                }));
                
                _gameProxy.IsItemInHandsNull.Value = true;
                _gameProxy.InHandsItemTypeId.Value = "";
                _gameProxy.InHandsMarkId = "";
                
                return true;
            }
            if (_gameProxy.IsItemInHandsNull.Value == false &&
                string.IsNullOrEmpty(_gameProxy.InHandsItemTypeId.Value) == false)
            {
                _gameProxy.ItemsUpCar.Add(new ItemUpCarProxy(new ItemUpCar()
                {
                    ItemTypeId = _gameProxy.InHandsItemTypeId.Value,
                    DestinationId = _gameProxy.InHandsMarkId,
                }));
                
                _gameProxy.IsItemInHandsNull.Value = true;
                _gameProxy.InHandsItemTypeId.Value = "";
                _gameProxy.InHandsMarkId = "";
                
                return true;
            }*/
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
                }
            }
            else //что-то есть
            {
                if (string.IsNullOrEmpty(_gameProxy.InHandsItemTypeId.Value)) return false;
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
    }
}