using Messages;
using ObservableCollections;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Services;
using UnityEngine;

namespace Items
{
    public class ItemsSystem
    {
        private readonly MessageSystem _messageSystem;
        private DropSettings _dropSettings { get; }
        private GameProxy _gameProxy { get; }
        
        
        public ItemsSystem(GameStateProvider gameStateProvider, DropSettings dropSettings, MessageSystem messageSystem)
        {
            _messageSystem = messageSystem;
            _dropSettings = dropSettings;
            _gameProxy = gameStateProvider.GameProxy;
        }

        public bool TryPickUp(ItemProxy itemProxy)
        {
            if (_gameProxy.IsItemInHandsNull.Value)
            {
                var removeResult = _gameProxy.ItemsInWorld.Remove(itemProxy);
                if (removeResult)
                {
                    _gameProxy.IsItemInHandsNull.Value = false;
                    _gameProxy.InHandsItemTypeId.Value = itemProxy.ItemTypeId;
                    _gameProxy.InHandsDestinationId = itemProxy.DestinationId;
                    _messageSystem.SendCompositeMessage(this, "pickup:" + itemProxy.DestinationId);
                    return true;
                }
            }
            return false;
        }

        public bool TryDrop()
        {
            if (_gameProxy.IsItemInHandsNull.Value == false)
            {
                _gameProxy.ItemsInWorld.Add(new ItemProxy(new ItemData
                {
                    ItemTypeId = _gameProxy.InHandsItemTypeId.Value,
                    DestinationId = _gameProxy.InHandsDestinationId,
                    Position = _dropSettings.GetDropPosition(),
                    Rotation = _dropSettings.GetDropRotation(),
                    LinearVelocity = _dropSettings.GetDropLinearVelocity(),
                    AngularVelocity = _dropSettings.GetDropAngularVelocity(),
                }));
                
                _messageSystem.SendCompositeMessage(this, "drop:" + _gameProxy.InHandsDestinationId);
                
                _gameProxy.IsItemInHandsNull.Value = true;
                _gameProxy.InHandsItemTypeId.Value = "";
                _gameProxy.InHandsDestinationId = "";
                
                return true;
            }
            return false;
        }

        public bool TrySpecificDrop(Vector3 endPos, float yOffset)
        {
            if (_gameProxy.IsItemInHandsNull.Value == false)
            {
                var velocity = GetVelocityParabocal(_dropSettings.GetDropPosition(), endPos, yOffset);
                
                _gameProxy.ItemsInWorld.Add(new ItemProxy(new ItemData
                {
                    ItemTypeId = _gameProxy.InHandsItemTypeId.Value,
                    DestinationId = _gameProxy.InHandsDestinationId,
                    Position = _dropSettings.GetDropPosition(),
                    Rotation = _dropSettings.GetDropRotation(),
                    LinearVelocity = velocity,
                    AngularVelocity = Vector3.zero,
                }));
                
                _messageSystem.SendCompositeMessage(this, "throw_up:" + _gameProxy.InHandsDestinationId);
                _messageSystem.SendCompositeMessage(this, "drop:" + _gameProxy.InHandsDestinationId);
                
                _gameProxy.IsItemInHandsNull.Value = true;
                _gameProxy.InHandsItemTypeId.Value = "";
                _gameProxy.InHandsDestinationId = "";
                
                return true;
            }
            return false;
        }

        public bool TryPutCat()
        {
            if (_gameProxy.IsItemInHandsNull.Value == false)
            {
                if (_gameProxy.InHandsItemTypeId.Value == "cat")
                {
                    _gameProxy.IsItemInHandsNull.Value = true;
                    _gameProxy.InHandsItemTypeId.Value = "";
                    _gameProxy.InHandsDestinationId = "";
                    
                    _gameProxy.IsCatInCar.Value = true;
                    
                    _messageSystem.SendCompositeMessage(this, "put_cat");
                    
                    return true;
                }
            }
            return false;
        }
        
        private Vector3 GetVelocityParabocal(Vector3 pointStart, Vector3 dropPointEnd, float heightOffset)
        {
            float gravity = Mathf.Abs(Physics.gravity.y);

            // высота апекса
            float apexY = dropPointEnd.y + heightOffset;

            // высота подъема от старта
            float heightToApex = apexY - pointStart.y;

            if (heightToApex <= 0f)
                heightToApex = 0.01f;

            // начальная вертикальная скорость
            float velocityY = Mathf.Sqrt(2f * gravity * heightToApex);

            // время подъема
            float timeUp = velocityY / gravity;

            // время падения от апекса до цели
            float heightDown = apexY - dropPointEnd.y;
            float timeDown = Mathf.Sqrt(2f * heightDown / gravity);

            float totalTime = timeUp + timeDown;

            // горизонтальное смещение
            Vector3 horizontalDelta = new Vector3(
                dropPointEnd.x - pointStart.x,
                0f,
                dropPointEnd.z - pointStart.z
            );

            // горизонтальная скорость
            Vector3 velocityXZ = horizontalDelta / totalTime;

            // итоговый вектор скорости
            Vector3 resultVelocity = new Vector3(
                velocityXZ.x,
                velocityY,
                velocityXZ.z
            );

            return resultVelocity;
        }
    }
}