using Paws_Parcels.Scripts.Saves.States.GameState;

namespace Car_Player
{
    public class DoorService
    {
        private GameProxy _gameProxy;
        
        public DoorService(GameStateProvider gameStateProvider)
        {
            _gameProxy = gameStateProvider.GameProxy;
        }
        
        public void ChangeDoor()
        {
            _gameProxy.IsCarDoorOpen.Value = !_gameProxy.IsCarDoorOpen.Value;
        }
    }
}