using R3;

namespace Paws_Parcels.Scripts.Saves.States
{
    public class GameSettings
    {
        private readonly GameSettingsState _gameSettingsState;
        
        public ReactiveProperty<float> MasterVolume { get; }
        public ReactiveProperty<int> Language { get; }
        
        public GameSettings(GameSettingsState gameSettingsState)
        {
            _gameSettingsState = gameSettingsState;
            
            MasterVolume = new ReactiveProperty<float>(gameSettingsState.MasterVolume);
            Language = new ReactiveProperty<int>(gameSettingsState.Language);
            
            MasterVolume.Skip(1).Subscribe(value => gameSettingsState.MasterVolume = value);
            Language.Skip(1).Subscribe(value => gameSettingsState.Language = value);
        }

        public void Change(GameSettingsState gameSettingsState)
        {
            MasterVolume.Value = gameSettingsState.MasterVolume;
            Language.Value = gameSettingsState.Language;
        }
        
        public void Change(GameSettings gameSettings)
        {
            MasterVolume.Value = gameSettings.MasterVolume.Value;
            Language.Value = gameSettings.Language.Value;
        }
    }
}