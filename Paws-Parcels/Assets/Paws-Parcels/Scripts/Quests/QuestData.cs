using System;
using System.Collections.Generic;

namespace Paws_Parcels.Scripts.Quests
{
    [Serializable]
    public class QuestData
    {
        public string Id;
        public int CurrentMission;
        public bool IsActiveDestination;
        //public bool IsManyMessages;
        //public MessageToCompleteProgress Message;
        //public List<MessageToCompleteProgress> ManyMessages;
        //public List<string> CurrentActiveDestinations;
    }
}