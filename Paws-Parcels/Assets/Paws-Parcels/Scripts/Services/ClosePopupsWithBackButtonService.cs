using System;
using R3;
using UnityEngine.InputSystem;

namespace Services
{
    public class ClosePopupsWithBackButtonService : IDisposable
    {
        private readonly GameInputService _gameInputService;
        public Observable<Unit> ClosePopupSignal => _closePopupSubject;
        
        private readonly Subject<Unit> _closePopupSubject = new();

        public ClosePopupsWithBackButtonService(GameInputService gameInputService)
        {
            _gameInputService = gameInputService;
            gameInputService.InputSystem.UI.Back.performed += Back;
        }

        private void Back(InputAction.CallbackContext obj)
        {
            _closePopupSubject.OnNext(Unit.Default);
        }

        public void Dispose()
        {
            _gameInputService.InputSystem.UI.Back.performed -= Back;
            _closePopupSubject?.Dispose();
        }
    }
}