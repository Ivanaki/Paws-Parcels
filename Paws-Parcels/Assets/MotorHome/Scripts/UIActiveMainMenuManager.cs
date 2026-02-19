using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace MotorHome.Scripts
{
    public class UIActiveMainMenuManager : IInitializable, IDisposable, IUIActiveCoreManager
    {
        public UIEnum LastUI { private set; get; }
        public ReactiveProperty<UIEnum> CurrentUI { set; get; } = new(UIEnum.Pause);

        private UIEnum _currentUIMy = UIEnum.Game;
        private UIEnum CurrentUIMy
        {
            set
            {
                LastUI = _currentUIMy;
                _currentUIMy = value;
            }
        }
        
        private readonly InputActionAsset _inputActionAsset;
        
        private InputAction _pauseActionUI;
        
        private IDisposable _disposable;
       

        public UIActiveMainMenuManager(InputActionAsset  inputActionAsset)
        {
            _inputActionAsset = inputActionAsset;
        }

        public void Initialize()
        {
            _pauseActionUI = InputSystem.actions.FindAction(ActionMaps.UI + "/Pause");
            
            _inputActionAsset.FindActionMap(ActionMaps.Player).Disable();
            _inputActionAsset.FindActionMap(ActionMaps.Car).Disable();
            _inputActionAsset.FindActionMap(ActionMaps.AllState).Disable();
            _inputActionAsset.FindActionMap(ActionMaps.UI).Enable();
            
            _pauseActionUI.performed += UIEscaped;
            
            _disposable = CurrentUI.Subscribe(uiEnum => CurrentUIMy = uiEnum);
        }
        
        private void UIEscaped(InputAction.CallbackContext context)
        {
            
            UIEscaped();
        }
        
        private void UIEscaped()
        {
            if (CurrentUI.Value is UIEnum.LoadPanel)
            {
                CurrentUI.Value = UIEnum.Pause;
            }
            else if(CurrentUI.Value is UIEnum.SettingsApply)
            {
                CurrentUI.Value = UIEnum.Settings;
            }
            else if (CurrentUI.Value is UIEnum.Settings)
            {
                CurrentUI.Value = UIEnum.SettingsApply;
            }
        }
            
            
        public void Dispose()
        {
            _pauseActionUI.performed -= UIEscaped;
            _disposable?.Dispose();
        }
    }
}