using System;

namespace Paws_Parcels.Scripts.Saves.States
{
    [Serializable]
    public class SettingsState
    {
        public float MasterVolume;
        public float MusicVolume = 1f;
        public float PlayerSensitivity;
        public float CarSensitivity;
        public string LanguageCode;

        //public float SoundEffectsVolume;
        //public float DialogsVolume;
        
        /*public SettingsState()
        {
            
        }

        public SettingsState(SettingsState settingsStateCopy)
        {
            MasterVolume = settingsStateCopy.MasterVolume;
            LanguageCode = settingsStateCopy.LanguageCode;
        }*/
    }
}
