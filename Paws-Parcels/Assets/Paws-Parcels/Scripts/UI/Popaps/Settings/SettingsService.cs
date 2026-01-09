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

            MasterVolume = settingsStateProvider.Settings.MasterVolume;
            Language = settingsStateProvider.Settings.Language;
        }

        public void SaveSettings(GameSettingsState gameSettingsState)
        {
            _settingsStateProvider.Settings.Change(gameSettingsState);
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