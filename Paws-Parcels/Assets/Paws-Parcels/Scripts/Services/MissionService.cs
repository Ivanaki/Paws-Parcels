using UnityEngine;

namespace Paws_Parcels.Scripts.Quests
{
    [System.Serializable]
    public class Mission
    {
        public string NameCode;
        public string DescriptionCode;
        public string DestinationNameCode;
        public string CargoNameCode;
        public Vector3 Position;
        public int Price;
    }
    
    public class MissionService
    {
        public string GetMissionName(int missionId)
        {
            return "testMissionName";
        }
    }
}