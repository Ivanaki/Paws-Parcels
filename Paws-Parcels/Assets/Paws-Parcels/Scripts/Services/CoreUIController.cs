using System;
using Car_Player;
using Messages;
using R3;
using UI.Core;
using UnityEngine.InputSystem;

namespace Services
{
    public class CoreUIController : IDisposable
    {
        private readonly GameInputService _gameInputService;
        private readonly CoreUIManager _coreUIManager;
        private readonly PutInCarItemsService _putInCarItemsService;

        private CompositeDisposable _disposables = new();
        
        public CoreUIController(GameInputService gameInputService, CoreUIManager coreUIManager, 
            PutInCarItemsService putInCarItemsService, ShowableInterfaceService showableInterfaceService,
            MessageSystem messageSystem)
        {
            _gameInputService = gameInputService;
            _coreUIManager = coreUIManager;
            _putInCarItemsService = putInCarItemsService;

            gameInputService.InputSystem.UI.Back.performed += BackUI;
            gameInputService.InputSystem.UI.CloseMap.performed += CloseMap;
            gameInputService.InputSystem.Gameplay.OpenPause.performed += OpenPause;
            gameInputService.InputSystem.Gameplay.OpenQuests.performed += OpenQuests;
            gameInputService.InputSystem.Gameplay.OpenMap.performed += OpenMap;

            _disposables.Add(showableInterfaceService.CloseAllObservable.Subscribe(_ => _coreUIManager.CloseAll())); 
            _disposables.Add(showableInterfaceService.BanOpenObservable.Subscribe(_ => _coreUIManager.BanOpen()));
            _disposables.Add(showableInterfaceService.UnbanOpenObservable.Subscribe(_ => _coreUIManager.UnbanOpen()));
            _disposables.Add(showableInterfaceService.OpenDialogObservable.Subscribe(_ => _coreUIManager.OpenScreenDialog()));
            
            _disposables.Add(messageSystem.AddListenerWithManyMessages("on_open_quests").Subscribe(x =>
            {
                OpenQuests();
            }));
            _disposables.Add(messageSystem.AddListenerWithManyMessages("on_open_map").Subscribe(x =>
            {
                OpenMap();
            }));
        }
        
        private void BackUI(InputAction.CallbackContext obj = new())
        {
            var result = _coreUIManager.TryCloseLastPopup();

            if (result == false)
            {
                _coreUIManager.TryCloseScreen();
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
        
        private void OpenMap(InputAction.CallbackContext obj = new())
        {
            _coreUIManager.OpenScreenMap();
        }
        
        private void CloseMap(InputAction.CallbackContext obj = new())
        {
            _coreUIManager.CloseScreenMap();
        }
        
        public void Dispose()
        {
            _gameInputService.InputSystem.UI.Back.performed -= BackUI;
            _gameInputService.InputSystem.UI.CloseMap.performed -= CloseMap;
            _gameInputService.InputSystem.Gameplay.OpenPause.performed -= OpenPause;
            _gameInputService.InputSystem.Gameplay.OpenQuests.performed -= OpenQuests;
            _gameInputService.InputSystem.Gameplay.OpenMap.performed -= OpenMap;
            _disposables?.Dispose();
        }
    }
}