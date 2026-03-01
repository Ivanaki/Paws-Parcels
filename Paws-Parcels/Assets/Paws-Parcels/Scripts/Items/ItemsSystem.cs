using ObservableCollections;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;

namespace Items
{
    public class ItemsSystem
    {
        private DropSettings _dropSettings { get; }
        private GameProxy _gameProxy { get; }
        
        
        public ItemsSystem(GameStateProvider gameStateProvider, DropSettings dropSettings)
        {
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
                    _gameProxy.InHandsMarkId = itemProxy.DestinationId;
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
                    DestinationId = _gameProxy.InHandsMarkId,
                    Position = _dropSettings.GetDropPosition(),
                    Rotation = _dropSettings.GetDropRotation(),
                    LinearVelocity = _dropSettings.GetDropLinearVelocity(),
                    AngularVelocity = _dropSettings.GetDropAngularVelocity(),
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