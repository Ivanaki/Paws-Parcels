using System;

namespace Paws_Parcels.Scripts.Saves.States.GameState
{
    [System.Serializable]
    public class GameMeta
    {
        public Guid SaveId;
        public string SaveName; //используется в названии папки
        public DateTime SaveDataTimeUtc;
        public string TrackedMissionCode;
        public bool IsAutoSave;
        public int GameVersion;
        public int SaveVersion;
    }
}