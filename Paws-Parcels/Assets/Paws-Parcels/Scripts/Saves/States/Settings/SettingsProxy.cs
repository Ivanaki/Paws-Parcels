using R3;

namespace Paws_Parcels.Scripts.Saves.States
{
    public class SettingsProxy
    {
        public readonly SettingsState SettingsState;
        
        public ReactiveProperty<float> MasterVolume { get; }
        public ReactiveProperty<float> PlayerSensitivity { get; }
        public ReactiveProperty<float> CarSensitivity { get; }
        public ReactiveProperty<string> LanguageCode { get; }
        
        public SettingsProxy(SettingsState settingsState)
        {
            SettingsState = settingsState;
            
            MasterVolume = new ReactiveProperty<float>(settingsState.MasterVolume);
            PlayerSensitivity = new ReactiveProperty<float>(settingsState.PlayerSensitivity);
            CarSensitivity = new ReactiveProperty<float>(settingsState.CarSensitivity);
            LanguageCode = new ReactiveProperty<string>(settingsState.LanguageCode);
            
            MasterVolume.Skip(1).Subscribe(value => settingsState.MasterVolume = value);
            PlayerSensitivity.Skip(1).Subscribe(value => settingsState.PlayerSensitivity = value);
            CarSensitivity.Skip(1).Subscribe(value => settingsState.CarSensitivity = value);
            LanguageCode.Skip(1).Subscribe(value => settingsState.LanguageCode = value);
        }

        public void Change(SettingsState settingsState)
        {
            MasterVolume.Value = settingsState.MasterVolume;
            PlayerSensitivity.Value = settingsState.PlayerSensitivity;
            CarSensitivity.Value = settingsState.CarSensitivity;
            LanguageCode.Value = settingsState.LanguageCode;
        }
        
        /*public void Change(SettingsProxy settingsProxy)
        {
            MasterVolume.Value = settingsProxy.MasterVolume.Value;
            LanguageCode.Value = settingsProxy.LanguageCode.Value;
        }*/
    }
}