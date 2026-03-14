using R3;

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
        
        public ReadOnlyReactiveProperty<bool> CutscenePlaying => _cutscenePlaying;        
        private ReactiveProperty<bool> _cutscenePlaying = new(false);
        
        public void CutscenePlay()
        {
            _cutscenePlaying.Value = true;
        }
        
        public void CutsceneStopped()
        {
            _cutscenePlaying.Value = false;
        }
    }
}