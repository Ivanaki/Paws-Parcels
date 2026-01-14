using System.Collections.Generic;
using Paws_Parcels.Scripts.Saves.States;
using R3;

namespace UI.Screens.Settings
{
    public class SettingsService
    {
        public ReadOnlyReactiveProperty<float> MasterVolume { get; }
        public ReadOnlyReactiveProperty<int> Language { get; }
        
        private readonly SettingsStateProvider _settingsStateProvider;

        public SettingsService(SettingsStateProvider settingsStateProvider)
        {
            _settingsStateProvider = settingsStateProvider;

            MasterVolume = settingsStateProvider.SettingsProxy.MasterVolume;
            Language = settingsStateProvider.SettingsProxy.Language;
        }

        public void SaveSettings(SettingsState settingsState)
        {
            _settingsStateProvider.SettingsProxy.Change(settingsState);
            _settingsStateProvider.SaveSettingsState();
        }
        
        public void ResetSettings()
        {
            _settingsStateProvider.ResetSettingsState();
        }

        public List<string> GetLanguages()
        {
            //TODO
            return new List<string>{"a", "b", "c"};
        }
    }
}