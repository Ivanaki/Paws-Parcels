using UnityEngine;

namespace Paws_Parcels.Scripts.Quests
{
    [System.Serializable]
    public class QuestData
    {
        public string QuestId;
        public string Title;
        public string Description;
        public Vector3 Position;
        public int Price;
    }
    
    public class QuestsService
    {
        public string GetQuestCode(int questId)
        {
            return "testMissionName";
        }
    }
}