using System;
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
                    _gameProxy.ItemsInWorld.Add(new(x.ItemData));
                }));
            });
        }
        
        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}