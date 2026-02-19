using R3;

namespace Paws_Parcels.Scripts.Saves.States
{
    public class SettingsProxy
    {
        public readonly SettingsState SettingsState;
        
        public ReactiveProperty<float> MasterVolume { get; }
        public ReactiveProperty<float> Sensitivity { get; }
        public ReactiveProperty<string> LanguageCode { get; }
        
        public SettingsProxy(SettingsState settingsState)
        {
            SettingsState = settingsState;
            
            MasterVolume = new ReactiveProperty<float>(settingsState.MasterVolume);
            Sensitivity = new ReactiveProperty<float>(settingsState.Sensitivity);
            LanguageCode = new ReactiveProperty<string>(settingsState.LanguageCode);
            
            MasterVolume.Skip(1).Subscribe(value => settingsState.MasterVolume = value);
            Sensitivity.Skip(1).Subscribe(value => settingsState.Sensitivity = value);
            LanguageCode.Skip(1).Subscribe(value => settingsState.LanguageCode = value);
        }

        public void Change(SettingsState settingsState)
        {
            MasterVolume.Value = settingsState.MasterVolume;
            Sensitivity.Value = settingsState.Sensitivity;
            LanguageCode.Value = settingsState.LanguageCode;
        }
        
        /*public void Change(SettingsProxy settingsProxy)
        {
            MasterVolume.Value = settingsProxy.MasterVolume.Value;
            LanguageCode.Value = settingsProxy.LanguageCode.Value;
        }*/
    }
}