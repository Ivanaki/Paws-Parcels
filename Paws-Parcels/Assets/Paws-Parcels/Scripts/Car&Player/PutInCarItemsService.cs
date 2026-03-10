using Items;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Unity.VisualScripting;

namespace Car_Player
{
    public class PutInCarItemsService
    {
        private GameProxy _gameProxy;

        public PutInCarItemsService(GameStateProvider gameStateProvider)
        {
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
            return false;
        }
    }
}