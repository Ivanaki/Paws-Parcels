using UnityEngine;

namespace MotorHome.Scripts
{
    [System.Serializable]
    public class QuestDataMini
    {
        [field: SerializeField] public QuestEnum Quest { get; private set; }
        [field: SerializeField] public int Money { get; private set; }
        
        public QuestDataMini(QuestEnum quest, int money)
        {
            Quest = quest;
            Money = money;
        }
    }
}