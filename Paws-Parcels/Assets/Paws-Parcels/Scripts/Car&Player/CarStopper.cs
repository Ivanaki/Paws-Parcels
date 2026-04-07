using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Services;

namespace Car_Player
{
    public class CarStopper
    {
        public CarStopper(MessageSystem messageSystem, GameProxy gameProxy)
        {
            messageSystem.AddListenerWithManyMessages("stop_car").Subscribe(_ =>
            {
                gameProxy.StopCar.Value = true;
            });
            
            messageSystem.AddListenerWithManyMessages("start_car").Subscribe(_ =>
            {
                gameProxy.StopCar.Value = false;
            });
        }
    }
}