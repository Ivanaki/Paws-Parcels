using System;

namespace Paws_Parcels.Scripts.Quests
{
    [Serializable]
    public class MessageToCompleteProgress
    {
        public bool IsSuccess;
        public bool IsCounted;
        public int ProgressCount;
    }
}