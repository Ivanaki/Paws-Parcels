using UnityEngine;

namespace MotorHome.Scripts.ScriptableObjects
{
    [CreateAssetMenu]
    public class QuestsSO : ScriptableObject
    {
        [field: SerializeField] public QuestDataMini[] Quests { get; private set; }
    }
}