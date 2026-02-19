

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
        public ReadOnlyReactiveProperty<float> Sensitivity => _sensitivity;
        public ReadOnlyReactiveProperty<float> Sensitivity01 => _sensitivity01;
        public ReadOnlyReactiveProperty<string> LanguageCode => _languageCode;
        public List<SettingsService.LocaleData> LanguageVariants { get; }


        private readonly ReactiveProperty<bool> _hasChanges = new(false);
        private readonly ReactiveProperty<float> _masterVolume = new();
        private readonly ReactiveProperty<float> _sensitivity = new();
        private readonly ReactiveProperty<float> _sensitivity01 = new();
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
            settingsService.Sensitivity.Subscribe(value => _sensitivity.Value = value);
            settingsService.Sensitivity.Subscribe(value => _sensitivity01.Value = value/_maxSensitivityOnSlider);
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
        
        public void RequestSetSensitivityBySlider(float value)
        {
            _hasChanges.Value = true;
            _sensitivity.Value = value * _maxSensitivityOnSlider;
            _sensitivity01.Value = value;
        }
        
        public void RequestSetSensitivityByInputField(string value)
        {
            _hasChanges.Value = true;
            if (float.TryParse(value, out float v))
            {
                v = Mathf.Clamp(v, 0f, _maxSensitivity);
                _sensitivity.Value = v;
                _sensitivity01.Value = v/_maxSensitivityOnSlider;
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
                    Sensitivity = _sensitivity.Value
                });
                _hasChanges.Value = false;
            }
        }

        public void RequestReset()
        {
            _settingsService.ResetSettings();
            _hasChanges.Value = false;
        }

        public void RequestCloseChecker()
        {
            RequestClose();
        }
    }
}