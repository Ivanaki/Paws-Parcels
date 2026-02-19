using R3;

namespace MotorHome.Scripts.Saves
{
    public class SettingsStateProxy
    {
        public ReactiveProperty<float> MasterVolume { get; }
        
        public ReactiveProperty<float> CarAim { get; }
        public ReactiveProperty<float> PlayerAim { get; }
        
        public ReactiveProperty<LanguageEnum> Language { get; }

        public SettingsStateProxy(SettingsState settingsState)
        {
            MasterVolume = new ReactiveProperty<float>(settingsState.MasterVolume);
            CarAim = new ReactiveProperty<float>(settingsState.CarAim);
            PlayerAim = new ReactiveProperty<float>(settingsState.PlayerAim);
            Language = new ReactiveProperty<LanguageEnum>(settingsState.Language);
            
            MasterVolume.Skip(1).Subscribe(value => settingsState.MasterVolume = value);
            CarAim.Skip(1).Subscribe(value => settingsState.CarAim = value);
            PlayerAim.Skip(1).Subscribe(value => settingsState.PlayerAim = value);
            Language.Skip(1).Subscribe(value => settingsState.Language = value);
        }
    }
}