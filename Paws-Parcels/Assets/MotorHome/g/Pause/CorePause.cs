using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace MotorHome.Scripts
{
    public class CorePause : IInitializable, IDisposable
    {
        private readonly CoreUI _coreUI;
        private readonly TimeManager _timeManager;
        private readonly CursorManager _cursorManager;
        private readonly UIActiveCoreManager _uiActiveCoreManager;
        
        private IDisposable _disposable;

        public CorePause(InputActionAsset  inputActionAsset, CoreUI coreUI, TimeManager timeManger, 
            CursorManager cursorManager, UIActiveCoreManager uiActiveCoreManager)
        {
            _coreUI = coreUI;
            _timeManager = timeManger;
            _cursorManager = cursorManager;
            _uiActiveCoreManager = uiActiveCoreManager;
        }

        public void Initialize()
        {
            _disposable = _uiActiveCoreManager.CurrentUI.Subscribe(value =>
            {
                if (value == UIEnum.Pause)
                    ShowPause();
                else if (value == UIEnum.Game)
                    HidePause();
            });
            
            _coreUI.ReturnPressed.Subscribe(_ =>
            {
                if (_uiActiveCoreManager.CurrentUI.Value != UIEnum.Pause)
                {
                    Debug.LogError("UI is bad and need fix");
                    return;
                }
                _uiActiveCoreManager.CurrentUI.Value = UIEnum.Game;
            });
        }
        

        public void HidePause()
        {
            _timeManager.Go();
            _cursorManager.SetCursorStateLocked(true);
            _coreUI.Hide();
        }
        
        private void ShowPause()
        {
            _timeManager.Stop();
            _cursorManager.SetCursorStateLocked(false);
            _coreUI.Show();
        }
        
        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}