namespace Services
{
    public class GameInputService
    {
        public InputSystem_Actions InputSystem { get; }

        private bool _isNowPlayer = true;
        private bool _isBlocking = false;
        
        public GameInputService()
        {
            InputSystem = new InputSystem_Actions();
            InputSystem.Enable();
            
            InputSystem.Player.Enable();
            InputSystem.Car.Disable();
        }

        public void ChangePlayerAndCar(bool isPlayer)
        {
            _isNowPlayer = isPlayer;
            UpdatePlayerCarState();
        }
        
        public void BlockPlayerAndCarWithUi()
        {
            InputSystem.Player.Disable();
            InputSystem.Car.Disable();
            _isBlocking = true;
        }
        
        public void UnblockPlayerAndCarWithUi()
        {
            _isBlocking = false;
            UpdatePlayerCarState();
        }

        private void UpdatePlayerCarState()
        {
            if(_isBlocking) return;
            
            if (_isNowPlayer)
            {
                InputSystem.Player.Enable();
                InputSystem.Car.Disable();
            }
            else
            {
                InputSystem.Player.Disable();
                InputSystem.Car.Enable();
            }
        }
    }
}