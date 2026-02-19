using System;

namespace MotorHome.Scripts
{
    [Serializable]
    public class CoreSaveData
    {
        public int SaveNumber;
        public string MissionName;
        public DateTime Time;
        public string SaveType;

        public CoreSaveData(int saveNumber, string missionName, DateTime time, string saveType)
        {
            SaveNumber = saveNumber;
            MissionName = missionName;
            Time = time;
            SaveType = saveType;
        }
        public CoreSaveData(){}
    }
}