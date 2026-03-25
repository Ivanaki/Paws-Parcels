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
        
        public ReadOnlyReactiveProperty<bool> AllGameplayInput => _allGameplayInput;       
        public ReadOnlyReactiveProperty<bool> CutsceneInput => _cutsceneInput;       
        public ReadOnlyReactiveProperty<bool> DialogueInput => _dialogueInput;       
        
        private readonly ReactiveProperty<bool> _allGameplayInput = new(true);
        private readonly ReactiveProperty<bool> _cutsceneInput = new(false);
        private readonly ReactiveProperty<bool> _dialogueInput = new(false);

        public void SetAllGameplayInput(bool value)
        {
            _allGameplayInput.Value = value;
        }

        public void SetCutsceneInput(bool value)
        {
            _cutsceneInput.Value = value;
        }

        public void SetDialogueInput(bool value)
        {
            _dialogueInput.Value = value;
        }
    }
}