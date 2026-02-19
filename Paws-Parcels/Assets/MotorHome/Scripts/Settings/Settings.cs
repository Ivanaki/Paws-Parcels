using System;
using MotorHome.Scripts.Saves;
using R3;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class Settings : IInitializable, IDisposable
    {
        private readonly GameStateProvider _gameStateProvider;
        private readonly SettingsUI _settingsUI;
        private readonly DialogsLanguageManager _dialogsLanguageManager;

        private IDisposable _settingsSubscription;
        
        private ReactiveProperty<bool> _hasChanges = new(false);
        
        //private InputAction _pauseActionUI;
        
        private bool _canCloseSettings = false;
        private IUIActiveCoreManager _currentUIActiveCoreManager = null;

        public Settings(GameStateProvider gameStateProvider, SettingsUI settingsUI,
            DialogsLanguageManager dialogsLanguageManager)
        {
            _gameStateProvider = gameStateProvider;
            _settingsUI = settingsUI;
            _dialogsLanguageManager = dialogsLanguageManager;
        }

        public void BindOpenSettingsObservable(Observable<Unit> observable, IUIActiveCoreManager uiActiveCoreManager)
        {
            _settingsSubscription?.Dispose();
            
            _currentUIActiveCoreManager = uiActiveCoreManager;
            _settingsSubscription = _currentUIActiveCoreManager.CurrentUI.Subscribe(_ =>
            {
                if (_currentUIActiveCoreManager.LastUI == UIEnum.Settings &&
                    _currentUIActiveCoreManager.CurrentUI.Value == UIEnum.SettingsApply)
                {
                    CloseSettings();
                }

                if (_currentUIActiveCoreManager.LastUI == UIEnum.SettingsApply &&
                    _currentUIActiveCoreManager.CurrentUI.Value == UIEnum.Settings)
                {
                    a?.Dispose();
                    _canCloseSettings = true;
                    _settingsUI.CloseCheckChangesUI();
                }
            });
            
            var disposable = observable.Subscribe(_ =>
            {
                if (uiActiveCoreManager.CurrentUI.Value == UIEnum.Pause)
                {
                    uiActiveCoreManager.CurrentUI.Value = UIEnum.Settings;
                    SettingsAfterPause();
                }
                else
                {
                    Debug.LogError("UI need fix");
                }
            });
            
            _settingsSubscription = Disposable.Combine(_settingsSubscription, disposable);
        }

        private void SettingsAfterPause()
        {
            _canCloseSettings = true;
            _settingsUI.Show();
            _gameStateProvider.LoadSettingsState();
            ApplySettingsStateFromProvider();
        }
        

        private void ApplySettingsStateFromProvider()
        {
            _settingsUI.Volume.Value = _gameStateProvider.SettingsStateProxy.CurrentValue.MasterVolume.CurrentValue;
            _settingsUI.CarAim.Value = _gameStateProvider.SettingsStateProxy.CurrentValue.CarAim.CurrentValue;
            _settingsUI.PlayerAim.Value = _gameStateProvider.SettingsStateProxy.CurrentValue.PlayerAim.CurrentValue;
            _settingsUI.LanguageProperty.Value = _gameStateProvider.SettingsStateProxy.CurrentValue.Language.Value;
            _hasChanges.Value = false;
        }
        
        public void Initialize()
        {
            var l = _dialogsLanguageManager.GetAllLanguages();
            _settingsUI.SetLanguageOptions(l, l[0].Item1 );
            
            _gameStateProvider.LoadSettingsState();
            ApplySettingsStateFromProvider();
            
            _hasChanges.Subscribe(_settingsUI.UpdateSaveButtonChanges);
            
            _settingsUI.LanguageProperty.Skip(1).Subscribe(value =>
            {
                
                _hasChanges.Value = true;
            });
            _settingsUI.Volume.Skip(1).Subscribe(value =>
            {
                _gameStateProvider.SettingsStateProxy.CurrentValue.MasterVolume.Value = value;
                _hasChanges.Value = true;
            });
            _settingsUI.PlayerAim.Skip(1).Subscribe(value =>
            {
                _gameStateProvider.SettingsStateProxy.CurrentValue.PlayerAim.Value = value;
                _hasChanges.Value = true;
            });
            _settingsUI.CarAim.Skip(1).Subscribe(value =>
            {
                _gameStateProvider.SettingsStateProxy.CurrentValue.CarAim.Value = value;
                _hasChanges.Value = true;
            });
            _settingsUI.SaveButtonPressed.Subscribe(_ =>
            {
                if (_hasChanges.Value)
                {
                    _gameStateProvider.SettingsStateProxy.CurrentValue.MasterVolume.Value = _settingsUI.Volume.Value;
                    _gameStateProvider.SettingsStateProxy.CurrentValue.Language.Value = _settingsUI.LanguageProperty.Value;
                    _gameStateProvider.SaveSettingsState();
                    _hasChanges.Value = false;
                }
            });
            _settingsUI.ResetButtonPressed.Subscribe(_ =>
            {
                _gameStateProvider.ResetSettingsState();
                ApplySettingsStateFromProvider();
                
                _hasChanges.Value = false;
            });

            _settingsUI.ExitButtonPressed.Subscribe(_ => CloseSettings());
            
        }

        private IDisposable a = null;
        
        private void CloseSettings()
        {
            if(!_canCloseSettings) return;
            
            _canCloseSettings = false;
            
            if (_hasChanges.Value)
            {
                if (_currentUIActiveCoreManager.CurrentUI.Value is UIEnum.Settings)
                {
                    _currentUIActiveCoreManager.CurrentUI.Value = UIEnum.SettingsApply;
                }
                
                a = _settingsUI.NeedSaveChanges().Subscribe(value =>
                {
                    if (value)
                    {
                        _gameStateProvider.SettingsStateProxy.CurrentValue.Language.Value = _settingsUI.LanguageProperty.Value;
                        _gameStateProvider.SaveSettingsState();
                        _hasChanges.Value = false;
                    }
                    else
                    {
                        _gameStateProvider.LoadSettingsState();
                        ApplySettingsStateFromProvider();
                    }
                        
                    _settingsUI.Hide();
                    
                    _currentUIActiveCoreManager.CurrentUI.Value = UIEnum.Pause;
                    a?.Dispose();
                });
            }
            else
            {
                if (_currentUIActiveCoreManager.CurrentUI.Value is
                    UIEnum.Settings or UIEnum.SettingsApply)
                {
                    _settingsUI.Hide();
                    _currentUIActiveCoreManager.CurrentUI.Value = UIEnum.Pause;
                }
                else
                {
                    Debug.LogError("UI need fix");
                }
            }
        }
        
        public void Dispose()
        {
            _settingsSubscription?.Dispose();
            //_pauseActionUI.performed -= CloseSettingsAction;
        }
    }
}