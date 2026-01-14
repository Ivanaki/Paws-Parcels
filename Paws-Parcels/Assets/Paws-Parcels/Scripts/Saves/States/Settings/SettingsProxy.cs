using R3;

namespace Paws_Parcels.Scripts.Saves.States
{
    public class SettingsProxy
    {
        private readonly SettingsState _settingsState;
        
        public ReactiveProperty<float> MasterVolume { get; }
        public ReactiveProperty<int> Language { get; }
        
        public SettingsProxy(SettingsState settingsState)
        {
            _settingsState = settingsState;
            
            MasterVolume = new ReactiveProperty<float>(settingsState.MasterVolume);
            Language = new ReactiveProperty<int>(settingsState.Language);
            
            MasterVolume.Skip(1).Subscribe(value => settingsState.MasterVolume = value);
            Language.Skip(1).Subscribe(value => settingsState.Language = value);
        }

        public void Change(SettingsState settingsState)
        {
            MasterVolume.Value = settingsState.MasterVolume;
            Language.Value = settingsState.Language;
        }
        
        public void Change(SettingsProxy settingsProxy)
        {
            MasterVolume.Value = settingsProxy.MasterVolume.Value;
            Language.Value = settingsProxy.Language.Value;
        }
    }
}