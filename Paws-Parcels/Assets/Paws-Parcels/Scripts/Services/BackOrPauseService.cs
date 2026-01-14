using System;
using R3;
using UnityEngine.InputSystem;

namespace Services
{
    public class BackOrPauseService : IDisposable
    {
        public ReadOnlyReactiveProperty<bool> IsEscaped => _isEscaped;
        
        private readonly ReactiveProperty<bool> _isEscaped = new(false);

        public BackOrPauseService(GameInputService gameInputService)
        {
            //gameInputService.InputSystem.Gameplay.Pause.performed += GameEscaped;
            //gameInputService.InputSystem.UI.Pause.performed += UIEscaped;
        }

        private void GameEscaped(InputAction.CallbackContext context)
        {
            
        }
        
        private void UIEscaped(InputAction.CallbackContext context)
        {
            
        }
        
        
        
        
        
        public void Dispose()
        {
            //_pauseActionCore.performed -= GameEscaped;
            //_pauseActionUI.performed -= UIEscaped;
            //_disposable?.Dispose();
        }
    }
}