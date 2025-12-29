using System.Collections.Generic;
using UnityEngine;

namespace MotorHome.Scripts
{
    public class QuestTargets : MonoBehaviour
    {
        private readonly Dictionary<QuestEnum, Transform> _questTargets = new();
        
        [System.Serializable]
        private class TargetData
        {
            [field: SerializeField] public QuestEnum QuestEnum { get; private set; }
            [field: SerializeField] public Transform Transform { get; private set; }
        }
        
        [SerializeField] private TargetData[] _targetDatas;

        private void Awake()
        {
            foreach (var targetData in _targetDatas)
            {
                _questTargets.Add(targetData.QuestEnum, targetData.Transform);
            }
        }

        public Transform GetQuestTarget(QuestEnum questEnum)
        {
            var did = _questTargets.TryGetValue(questEnum, out var target);
            if (!did) return null;
            return target;
        }
    }
}