using Messages;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Services;

namespace Car_Player
{
    public class DoorService
    {
        private readonly MessageSystem _messageSystem;
        private GameProxy _gameProxy;
        
        public DoorService(GameStateProvider gameStateProvider, MessageSystem messageSystem)
        {
            _messageSystem = messageSystem;
            _gameProxy = gameStateProvider.GameProxy;
        }
        
        public void ChangeDoor()
        {
            _gameProxy.IsCarDoorOpen.Value = !_gameProxy.IsCarDoorOpen.Value;
            _messageSystem.SendCompositeMessage(this,
                _gameProxy.IsCarDoorOpen.Value ? "car_door_open" : "car_door_close");
        }
    }
}