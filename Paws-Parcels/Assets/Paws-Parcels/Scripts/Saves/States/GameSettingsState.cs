using System;

namespace Paws_Parcels.Scripts.Saves.States
{
    [Serializable]
    public class GameSettingsState
    {
        public float MasterVolume;
        public int Language;
        
        //public float MusicVolume;
        //public float SoundEffectsVolume;
        //public float DialogsVolume;
        
        public GameSettingsState()
        {
            
        }

        public GameSettingsState(GameSettingsState gameSettingsStateCopy)
        {
            MasterVolume = gameSettingsStateCopy.MasterVolume;
            Language = gameSettingsStateCopy.Language;
        }
    }
}