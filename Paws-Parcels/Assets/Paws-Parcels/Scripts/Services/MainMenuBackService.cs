using System;
using UI.Core;
using UI.MainMenu;
using UnityEngine.InputSystem;

namespace Services
{
    public class MainMenuBackService : IDisposable
    {
        private readonly GameInputService _gameInputService;
        private readonly MainMenuUIManager _mainMenuUIManager;


        public MainMenuBackService(GameInputService gameInputService, MainMenuUIManager mainMenuUIManager)
        {
            _gameInputService = gameInputService;
            _mainMenuUIManager = mainMenuUIManager;

            gameInputService.InputSystem.UI.Back.performed += BackUI;
        }
        
        private void BackUI(InputAction.CallbackContext obj = new())
        {
            _mainMenuUIManager.TryCloseLastPopup();
        }
        
        public void Dispose()
        {
            _gameInputService.InputSystem.UI.Back.performed -= BackUI;
        }
    }
}