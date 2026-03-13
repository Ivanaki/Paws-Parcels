using System;
using Car_Player;
using UI.Core;
using UnityEngine.InputSystem;

namespace Services
{
    public class CoreBackService : IDisposable
    {
        private readonly GameInputService _gameInputService;
        private readonly CoreUIManager _coreUIManager;
        private readonly PutInCarItemsService _putInCarItemsService;

        public CoreBackService(GameInputService gameInputService, CoreUIManager coreUIManager, 
            PutInCarItemsService putInCarItemsService)
        {
            _gameInputService = gameInputService;
            _coreUIManager = coreUIManager;
            _putInCarItemsService = putInCarItemsService;

            gameInputService.InputSystem.UI.Back.performed += BackUI;
            gameInputService.InputSystem.Gameplay.OpenPause.performed += OpenPause;
            gameInputService.InputSystem.Gameplay.OpenQuests.performed += OpenQuests;
        }
        
        private void BackUI(InputAction.CallbackContext obj = new())
        {
            var result = _coreUIManager.TryCloseLastPopup();

            if (result == false)
            {
                _coreUIManager.CloseScreen();
            }
        }

        private void OpenPause(InputAction.CallbackContext obj = new())
        {
            var resultPut = _putInCarItemsService.TryCancel();
            if(resultPut) return;
            
            _coreUIManager.OpenScreenPause();
        }

        private void OpenQuests(InputAction.CallbackContext obj = new())
        {
            _coreUIManager.OpenScreenQuests();
        }
        
        public void Dispose()
        {
            _gameInputService.InputSystem.UI.Back.performed -= BackUI;
            _gameInputService.InputSystem.Gameplay.OpenPause.performed -= OpenPause;
            _gameInputService.InputSystem.Gameplay.OpenQuests.performed -= OpenQuests;
        }
    }
}