using Messages;
using Paws_Parcels.Scripts.Saves.States.GameState;

namespace Car_Player
{
    public class CarGasStateListener
    {
        public CarGasStateListener(MessageSystem messageSystem, GameProxy gameProxy)
        {
            messageSystem.AddListenerWithManyMessages("stop_gas").Subscribe(_ =>
            {
                gameProxy.HasGas.Value = false;
            });

            messageSystem.AddListenerWithManyMessages("start_gas").Subscribe(_ =>
            {
                gameProxy.HasGas.Value = true;
            });
        }
    }
}
