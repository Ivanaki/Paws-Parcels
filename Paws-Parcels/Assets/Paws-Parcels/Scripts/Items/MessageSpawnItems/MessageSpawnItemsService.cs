using System;
using System.Linq;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Services;
using UnityEngine;
using Zenject;

namespace Items.MessageSpawnItems
{
    public class MessageSpawnItemsService : IInitializable, IDisposable
    {
        private readonly MessageSystem _messageSystem;
        private readonly GameProxy _gameProxy;

        private readonly CompositeDisposable _disposables = new();

        public MessageSpawnItemsService(MessageSystem messageSystem, GameProxy gameProxy)
        {
            _messageSystem = messageSystem;
            _gameProxy = gameProxy;
        }
        
        public void Initialize()
        {
            var messageSpawnItemSO = Resources.Load<MessageSpawnItemsSO>("Editor/MessageSpawnItems");
            messageSpawnItemSO.ItemDataWithMessages.ForEach(x =>
            {
                _disposables.Add(_messageSystem.AddListenerWithManyMessages(x.Message).Subscribe(_ =>
                {
                    _gameProxy.ItemsInWorld.Add(new(new ItemData
                    {
                        ItemTypeId = x.ItemData.ItemTypeId,
                        DestinationId = x.ItemData.DestinationId,
                        Position = x.ItemData.Position,
                        Rotation = x.ItemData.Rotation,
                        LinearVelocity = x.ItemData.LinearVelocity,
                        AngularVelocity = x.ItemData.AngularVelocity
                    }));
                }));
            });

            messageSpawnItemSO.ItemRemoveWithMessages.ForEach(x =>
            {
                _disposables.Add(_messageSystem.AddListenerWithManyMessages(x.Message).Subscribe(_ =>
                {
                    var destinationId = x.DestinationId;

                    var item = _gameProxy.ItemsInWorld.FirstOrDefault(i => i.DestinationId == destinationId);
                    if (item != null)
                        _gameProxy.ItemsInWorld.Remove(item);

                    foreach (var putter in _gameProxy.ItemsDestinationsPutters)
                    {
                        var putterItem = putter.Value.Items.FirstOrDefault(i => i.DestinationId == destinationId);
                        if (putterItem != null)
                            putter.Value.Items.Remove(putterItem);
                    }
                }));
            });
        }
        
        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}