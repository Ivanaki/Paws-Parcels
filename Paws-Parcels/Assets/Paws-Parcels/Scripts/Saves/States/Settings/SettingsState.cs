using System;

namespace Paws_Parcels.Scripts.Saves.States
{
    [Serializable]
    public class SettingsState
    {
        public float MasterVolume;
        public float Sensitivity;
        public string LanguageCode;
        
        //public float MusicVolume;
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