using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace MotorHome.Scripts
{
    public interface IUIActiveCoreManager
    {
        UIEnum LastUI { get; }
        public ReactiveProperty<UIEnum> CurrentUI { get; set; }
    }

    public class UIActiveCoreManager : IInitializable, IDisposable, IUIActiveCoreManager
    {
        public UIEnum LastUI { private set; get; }
        public ReactiveProperty<UIEnum> CurrentUI { set; get; } = new(UIEnum.Game);

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
        
        private InputAction _pauseActionCore;
        private InputAction _pauseActionUI;
        
        private IDisposable _disposable;
       

        public UIActiveCoreManager(InputActionAsset  inputActionAsset)
        {
            _inputActionAsset = inputActionAsset;
        }

        public void Initialize()
        {
            _pauseActionCore = InputSystem.actions.FindAction(ActionMaps.AllState + "/Pause");
            _pauseActionUI = InputSystem.actions.FindAction(ActionMaps.UI + "/Pause");
            
            _pauseActionCore.performed += GameEscaped;
            _pauseActionUI.performed += UIEscaped;

            _disposable = CurrentUI.Subscribe(UpdateActionMaps);
            
            var dis = CurrentUI.Subscribe(uiEnum => CurrentUIMy = uiEnum);
            _disposable = Disposable.Combine(_disposable, dis);
        }

        private void GameEscaped(InputAction.CallbackContext context) => GameEscaped();
        
        private bool _flag = false;
        private void UIEscaped(InputAction.CallbackContext context)
        {
            if (!_flag)
            {
                _flag = true;
                return;
            }
            UIEscaped();
        }

        private void GameEscaped()
        {
            if (CurrentUI.Value != UIEnum.Game)
            {
                Debug.LogError("Current menu is not Game");
                return;
            }
            
            CurrentUI.Value = UIEnum.Pause;
        }
        
        private void UIEscaped()
        {
            if (CurrentUI.Value == UIEnum.Game)
            {
                Debug.LogError("Current menu is Game");
                return;
            }

            if (CurrentUI.Value is UIEnum.Pause or UIEnum.QuestsPanel)
            {
                CurrentUI.Value = UIEnum.Game;
            }
            else if (CurrentUI.Value is UIEnum.LoadPanel)
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

        private void UpdateActionMaps(UIEnum uiEnum)
        {
            switch (CurrentUI.Value)
            {
                case UIEnum.Game:
                    _inputActionAsset.FindActionMap(ActionMaps.AllState).Enable();
                    _inputActionAsset.FindActionMap(ActionMaps.UI).Disable();
                    break;
                case UIEnum.Pause:
                case UIEnum.Settings:
                case UIEnum.SettingsApply:
                case UIEnum.LoadPanel:
                case UIEnum.QuestsPanel:
                    _inputActionAsset.FindActionMap(ActionMaps.AllState).Disable();
                    _inputActionAsset.FindActionMap(ActionMaps.UI).Enable();
                    break;
            }
        }
            
            
        public void Dispose()
        {
            _pauseActionCore.performed -= GameEscaped;
            _pauseActionUI.performed -= UIEscaped;
            _disposable?.Dispose();
        }
    }

    public enum UIEnum
    {
        Game,
        Pause,
        Settings,
        SettingsApply,
        LoadPanel,
        QuestsPanel
    }
}