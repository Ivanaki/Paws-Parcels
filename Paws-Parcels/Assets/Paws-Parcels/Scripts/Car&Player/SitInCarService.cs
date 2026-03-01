using Paws_Parcels.Scripts.Saves.States.GameState;

namespace Paws_Parcels.Scripts.Car_Player
{
    public class SitInCarService
    {
        private GameProxy _gameProxy;
        
        public SitInCarService(GameStateProvider gameStateProvider)
        {
            _gameProxy = gameStateProvider.GameProxy;
        }

        public bool TrySit()
        {
            if (_gameProxy.IsPlayerInCar.Value == false && _gameProxy.IsItemInHandsNull.Value/* &&
                _gameProxy.IsCarDoorOpen.Value == false*/)
            {
                _gameProxy.IsPlayerInCar.Value = true;
                return true;
            }

            return false;
        }

        public bool TryExit()
        {
            if (_gameProxy.IsPlayerInCar.Value)
            {
                _gameProxy.IsPlayerInCar.Value = false;
                return true;
            }
            return false;
        }
    }
}