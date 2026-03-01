namespace Services
{
    public class GameInputService
    {
        public InputSystem_Actions InputSystem { get; }
        
        public GameInputService()
        {
            InputSystem = new InputSystem_Actions();
            InputSystem.Enable();
        }
    }
}