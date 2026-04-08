using System;
using System.Collections.Generic;

namespace Paws_Parcels.Scripts.Quests
{
    [Serializable]
    public class QuestData
    {
        public string Id;
        public int CurrentMission;
        public List<string> ActiveDestinations = new();
        public int MessageProgress;
        public bool HasMissionTimer;
        public float MissionTimerSeconds;
        public float MissionTimerTotalSeconds;
        //public List<string> MessagesProgres = new();
    }
}
