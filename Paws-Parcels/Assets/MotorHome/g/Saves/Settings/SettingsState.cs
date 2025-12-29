namespace MotorHome.Scripts.Saves
{
    [System.Serializable]
    public class SettingsState
    {
        //здесь будет словарь, в который все объекты будут добавлять данные
        
        
        //volume
        public float MasterVolume;
        
        //game settings
        public float CarAim;
        public float PlayerAim;
        
        public LanguageEnum Language;

        public SettingsState() { }

        public SettingsState(SettingsState copyState)
        {
            MasterVolume = copyState.MasterVolume;
            CarAim = copyState.CarAim;
            PlayerAim = copyState.PlayerAim;
            Language = copyState.Language;
        }
    }
}