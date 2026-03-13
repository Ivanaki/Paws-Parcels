

using System.Collections.Generic;
using ObservableCollections;
using Paws_Parcels.Scripts.Saves.States;
using R3;
using UI.MVVM;
using UnityEngine;

namespace UI.Screens.Settings
{
    public class PopupSettingsViewModel : WindowViewModel
    {
        public override string Id => "Settings";
        
        
        public ReadOnlyReactiveProperty<bool> HasChanges => _hasChanges;
        public ReadOnlyReactiveProperty<float> MasterVolume => _masterVolume;
        public ReadOnlyReactiveProperty<float> SensitivityPlayer => _sensitivityPlayer;
        public ReadOnlyReactiveProperty<float> SensitivityPlayer01 => _sensitivityPlayer01;
        public ReadOnlyReactiveProperty<float> SensitivityCar => _sensitivityCar;
        public ReadOnlyReactiveProperty<float> SensitivityCar01 => _sensitivityCar01;
        public ReadOnlyReactiveProperty<string> LanguageCode => _languageCode;
        public List<SettingsService.LocaleData> LanguageVariants { get; }


        private readonly ReactiveProperty<bool> _hasChanges = new(false);
        private readonly ReactiveProperty<float> _masterVolume = new();
        private readonly ReactiveProperty<float> _sensitivityPlayer = new();
        private readonly ReactiveProperty<float> _sensitivityPlayer01 = new();
        private readonly ReactiveProperty<float> _sensitivityCar = new();
        private readonly ReactiveProperty<float> _sensitivityCar01 = new();
        private readonly ReactiveProperty<string> _languageCode = new();
        
        private readonly SettingsService _settingsService;
        private readonly float _maxSensitivity;
        private readonly float _maxSensitivityOnSlider;


        public PopupSettingsViewModel(SettingsService settingsService, float maxSensitivity, float maxSensitivityOnSlider)
        {
            _settingsService = settingsService;
            _maxSensitivity = Mathf.Max(maxSensitivity, 1f);
            _maxSensitivityOnSlider = Mathf.Max(maxSensitivityOnSlider, 1f);

            settingsService.MasterVolume.Subscribe(value => _masterVolume.Value = value);
            settingsService.SensitivityPlayer.Subscribe(value => _sensitivityPlayer.Value = value);
            settingsService.SensitivityPlayer.Subscribe(value => _sensitivityPlayer01.Value = value/_maxSensitivityOnSlider);
            settingsService.SensitivityCar.Subscribe(value => _sensitivityCar.Value = value);
            settingsService.SensitivityCar.Subscribe(value => _sensitivityCar01.Value = value/_maxSensitivityOnSlider);
            settingsService.LanguageCode.Subscribe(value => _languageCode.Value = value);
            
            LanguageVariants = settingsService.GetLanguages();
        }

        public void RequestSetMasterVolumeBySlider(float volume)
        {
            _hasChanges.Value = true;
            _masterVolume.Value = volume;
        }
        
        public void RequestSetMasterVolumeByInputField(string volume)
        {
            _hasChanges.Value = true;
            if (float.TryParse(volume, out float v))
            {
                v = Mathf.Clamp01(v);
                _masterVolume.Value = v;
            }
        }
        
        public void RequestSetSensitivityPlayerBySlider(float value)
        {
            _hasChanges.Value = true;
            _sensitivityPlayer.Value = value * _maxSensitivityOnSlider;
            _sensitivityPlayer01.Value = value;
        }
        
        public void RequestSetSensitivityPlayerByInputField(string value)
        {
            _hasChanges.Value = true;
            if (float.TryParse(value, out float v))
            {
                v = Mathf.Clamp(v, 0f, _maxSensitivity);
                _sensitivityPlayer.Value = v;
                _sensitivityPlayer01.Value = v/_maxSensitivityOnSlider;
            }
        }

        public void RequestSetSensitivityCarBySlider(float value)
        {
            _hasChanges.Value = true;
            _sensitivityCar.Value = value * _maxSensitivityOnSlider;
            _sensitivityCar01.Value = value;
        }
        
        public void RequestSetSensitivityCarByInputField(string value)
        {
            _hasChanges.Value = true;
            if (float.TryParse(value, out float v))
            {
                v = Mathf.Clamp(v, 0f, _maxSensitivity);
                _sensitivityCar.Value = v;
                _sensitivityCar01.Value = v/_maxSensitivityOnSlider;
            }
        }

        public void RequestSetLanguage(string languageCode)
        {
            _hasChanges.Value = true;
            _languageCode.Value = languageCode;
        }

        public void RequestSave()
        {
            if (_hasChanges.Value)
            {
                _settingsService.SaveSettings(new SettingsState()
                {
                    MasterVolume = _masterVolume.Value,
                    LanguageCode = _languageCode.Value,
                    PlayerSensitivity = _sensitivityPlayer.Value,
                    CarSensitivity = _sensitivityCar.Value,
                });
                _hasChanges.Value = false;
            }
        }

        public void RequestReset()
        {
            _settingsService.ResetSettings();
            
            _masterVolume.Value = _settingsService.MasterVolume.CurrentValue;
            _sensitivityPlayer.Value = _settingsService.SensitivityPlayer.CurrentValue;
            _sensitivityPlayer01.Value = _settingsService.SensitivityPlayer.CurrentValue/_maxSensitivityOnSlider;
            _sensitivityCar.Value = _settingsService.SensitivityCar.CurrentValue;
            _sensitivityCar01.Value = _settingsService.SensitivityCar.CurrentValue/_maxSensitivityOnSlider;
            _languageCode.Value = _settingsService.LanguageCode.CurrentValue;
            _hasChanges.Value = false;
        }

        public void RequestCloseChecker()
        {
            RequestClose();
        }
    }
}