using Services;
using Zenject;

namespace Paws_Parcels.Scripts
{
    public class InputsMainMenuService : IInitializable
    {
        private InputSystem_Actions _inputSystem { get; }
        
        public InputsMainMenuService(GameInputService gameInputService)
        {
            _inputSystem = gameInputService.InputSystem;
        }
        
        public void Initialize()
        {
            _inputSystem.UI.Enable();
            
            _inputSystem.Gameplay.Disable();
            _inputSystem.CarNotMovement.Disable();
            _inputSystem.Car.Disable();
            _inputSystem.PlayerNotMovement.Disable();
            _inputSystem.Player.Disable();
        }
    }
}