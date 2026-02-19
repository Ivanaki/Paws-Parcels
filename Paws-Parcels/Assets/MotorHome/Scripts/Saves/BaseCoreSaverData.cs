using System.Collections.Generic;

namespace MotorHome.Scripts
{
    [System.Serializable]
    public class BaseCoreSaverData
    {
        public int SaveNumber;
        public Dictionary<int, CoreSaveData> SaveData;

        public BaseCoreSaverData(){}
        
        public BaseCoreSaverData(int saveNumber, Dictionary<int, CoreSaveData> saveData)
        {
            SaveNumber = saveNumber;
            SaveData = saveData;
        }
    }
}