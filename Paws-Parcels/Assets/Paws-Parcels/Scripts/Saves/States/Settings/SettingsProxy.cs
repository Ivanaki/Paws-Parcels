using R3;

namespace Paws_Parcels.Scripts.Saves.States
{
    public class SettingsProxy
    {
        private readonly SettingsState _settingsState;
        
        public ReactiveProperty<float> MasterVolume { get; }
        public ReactiveProperty<string> LanguageCode { get; }
        
        public SettingsProxy(SettingsState settingsState)
        {
            _settingsState = settingsState;
            
            MasterVolume = new ReactiveProperty<float>(settingsState.MasterVolume);
            LanguageCode = new ReactiveProperty<string>(settingsState.LanguageCode);
            
            MasterVolume.Skip(1).Subscribe(value => settingsState.MasterVolume = value);
            LanguageCode.Skip(1).Subscribe(value => settingsState.LanguageCode = value);
        }

        public void Change(SettingsState settingsState)
        {
            MasterVolume.Value = settingsState.MasterVolume;
            LanguageCode.Value = settingsState.LanguageCode;
        }
        
        public void Change(SettingsProxy settingsProxy)
        {
            MasterVolume.Value = settingsProxy.MasterVolume.Value;
            LanguageCode.Value = settingsProxy.LanguageCode.Value;
        }
    }
}