using System.Linq;
using Paws_Parcels.Scripts.Player;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Services;
using UnityEngine;

namespace Destinations
{
    public class ItemDestinationPutterService
    {
        private readonly MessageSystem _messageSystem;
        private readonly GameProxy _gameProxy;
        
        public ItemDestinationPutterService(GameStateProvider gameStateProvider, MessageSystem messageSystem)
        {
            _messageSystem = messageSystem;
            _gameProxy = gameStateProvider.GameProxy;
        }

        public bool TryPutOrTakeItemInDestination(string itemDestinationPutterId, int maxItems)
        {
            if (_gameProxy.IsItemInHandsNull.Value)
            {
                if (_gameProxy.ItemsDestinationsPutters.TryGetValue(itemDestinationPutterId,
                        out var itemDestinationProxy))
                {
                    if (itemDestinationProxy.Items.Count > 0)
                    {
                        var lastItem = itemDestinationProxy.Items.Last();
                        itemDestinationProxy.Items.Remove(lastItem);
                        _gameProxy.IsItemInHandsNull.Value = false;
                        _gameProxy.InHandsItemTypeId.Value = lastItem.ItemTypeId;
                        _gameProxy.InHandsDestinationId = lastItem.DestinationId;
                        
                        _messageSystem.SendCompositeMessage(this, itemDestinationPutterId + "-take:" + lastItem.DestinationId);
                        
                        return true;
                    }
                }
            }
            else //что-то есть
            {
                if (string.IsNullOrEmpty(_gameProxy.InHandsItemTypeId.Value))
                {
                    Debug.LogError("InHandsItemTypeId is null or empty");
                    return false;
                }

                if (_gameProxy.ItemsDestinationsPutters.ContainsKey(itemDestinationPutterId) == false) 
                    _gameProxy.ItemsDestinationsPutters.Add(itemDestinationPutterId,
                        new ItemDestinationProxy(new ItemDestinationData()));
                
                var itemDestinationProxy = _gameProxy.ItemsDestinationsPutters[itemDestinationPutterId];
                
                if(itemDestinationProxy.Items.Count == maxItems) return false;
                
                itemDestinationProxy.Items.Add(new(new ()
                {
                    ItemTypeId = _gameProxy.InHandsItemTypeId.Value,
                    DestinationId = _gameProxy.InHandsDestinationId
                }));
                
                _messageSystem.SendCompositeMessage(this, itemDestinationPutterId + "-put:" + _gameProxy.InHandsDestinationId);
                
                _gameProxy.IsItemInHandsNull.Value = true;
                _gameProxy.InHandsItemTypeId.Value = "";
                _gameProxy.InHandsDestinationId = "";
                
                
                
                return true;
            }

            return false;
        }
    }
}