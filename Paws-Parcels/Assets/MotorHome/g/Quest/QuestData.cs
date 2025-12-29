using UnityEngine;

namespace MotorHome.Scripts
{
    [System.Serializable]
    public class QuestData
    {
        [field: SerializeField] public QuestEnum Quest { get; private set; }
        [field: SerializeField] public string MainName { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public string Destination { get; private set; }
        [field: SerializeField] public string Cargo { get; private set; }

        public QuestData(QuestEnum quest, string mainName, string description, string destination, string cargo)
        {
            Quest = quest;
            MainName = mainName;
            Description = description;
            Destination = destination;
            Cargo = cargo;
        }
    }
}