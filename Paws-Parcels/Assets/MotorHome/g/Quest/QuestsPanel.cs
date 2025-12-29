using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace MotorHome.Scripts
{
    public class QuestsPanel : IInitializable, IDisposable
    {
       // public ReadOnlyReactiveProperty<bool> IsActive => _isActive;
        
        private readonly TimeManager _timeManager;
        private readonly InputActionAsset _inputActionAsset;
        private readonly CursorManager _cursorManager;
        private readonly UIActiveCoreManager _uiActiveCoreManager;
        //private readonly ReactiveProperty<bool> _isActive = new(false);
        
        private InputAction _questsActionCore;
        private InputAction _questsActionUI;

        private IDisposable _disposable;
        private CoreUI _coreUI;

        public QuestsPanel(TimeManager timeManager,  InputActionAsset  inputActionAsset,
            CursorManager cursorManager, UIActiveCoreManager uiActiveCoreManager, CoreUI coreUI)
        {
            _timeManager = timeManager;
            
            _inputActionAsset = inputActionAsset;
            _cursorManager = cursorManager;
            _uiActiveCoreManager = uiActiveCoreManager;
            _coreUI = coreUI;
        }
        
        public void Initialize()
        {
            _disposable = _coreUI.Helper.ReturnButtonPressed.Subscribe(_ =>
            {
                Debug.Log("Return button pressed");
                _uiActiveCoreManager.CurrentUI.Value = UIEnum.Game;
            });
            
            //_questsSystemUI._button.onClick.AddListener(() => Debug.Log("Return button pressed"));
            
            _questsActionCore = InputSystem.actions.FindAction(ActionMaps.AllState + "/QuestsPanel");
            _questsActionUI = InputSystem.actions.FindAction(ActionMaps.UI + "/QuestsPanel");

            _questsActionCore.performed += ShowPanel;
            _questsActionUI.performed += HidePanel;
            
            var dis = _uiActiveCoreManager.CurrentUI.Subscribe(value =>
            {
                
                //Debug.Log(value);
                
                if (value == UIEnum.Game)
                    HidePanel();
                else if (value == UIEnum.QuestsPanel)
                    ShowPanel();
            }); _disposable = Disposable.Combine(_disposable, dis);
        }

        private void ShowPanel(InputAction.CallbackContext obj)
        {
            if (_uiActiveCoreManager.CurrentUI.Value == UIEnum.Game)
            {
                _uiActiveCoreManager.CurrentUI.Value = UIEnum.QuestsPanel;
            }
        }

        private void HidePanel(InputAction.CallbackContext obj)
        {
            if (_uiActiveCoreManager.CurrentUI.Value == UIEnum.QuestsPanel)
            {
                _uiActiveCoreManager.CurrentUI.Value = UIEnum.Game;
            }
        }

        private void ShowPanel()
        {
            _cursorManager.SetCursorStateLocked(false);
            //_questsSystemUI.Open();
            _timeManager.Stop();
            //_isActive.Value = true;
        }

        private void HidePanel()
        {
            _cursorManager.SetCursorStateLocked(true);
            //_questsSystemUI.Close();
            _timeManager.Go();
            //_isActive.Value = false;
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}