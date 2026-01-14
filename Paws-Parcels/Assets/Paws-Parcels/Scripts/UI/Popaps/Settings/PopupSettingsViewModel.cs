

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
        public ReadOnlyReactiveProperty<int> Language => _language;
        public List<string> LanguageVariants { get; }


        private readonly ReactiveProperty<bool> _hasChanges = new(false);
        private readonly ReactiveProperty<float> _masterVolume = new();
        private readonly ReactiveProperty<int> _language = new();
        
        private readonly SettingsService _settingsService;
        

        public PopupSettingsViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;

            settingsService.MasterVolume.Subscribe(value => _masterVolume.Value = value);
            settingsService.Language.Subscribe(value => _language.Value = value);
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

        public void RequestSetLanguage(int language)
        {
            _hasChanges.Value = true;
            _language.Value = language;
        }

        public void RequestSave()
        {
            if (_hasChanges.Value)
            {
                _settingsService.SaveSettings(new SettingsState()
                {
                    MasterVolume = _masterVolume.Value,
                    Language = _language.Value
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