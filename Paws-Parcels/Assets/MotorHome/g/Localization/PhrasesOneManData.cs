using UnityEngine;

namespace MotorHome.Scripts
{
    [System.Serializable]
    public class PhrasesOneManData
    {
        public enum WhoTalk
        {
            Player,
            Npc
        }
        
        [field: SerializeField] public WhoTalk WhoTalking { get; private set; }
        [field: SerializeField, TextArea(1,5)] public string[] Phrases { get; private set; }
    }
}