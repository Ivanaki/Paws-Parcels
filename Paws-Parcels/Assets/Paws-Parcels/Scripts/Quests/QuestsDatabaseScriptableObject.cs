using System.Collections.Generic;
using Paws_Parcels.Scripts.Quests;
using UnityEngine;

namespace Quests
{
    [CreateAssetMenu(fileName = "QuestsDatabase", menuName = "Paws_parcels/QuestsDatabase")]
    public class QuestsDatabaseScriptableObject : ScriptableObject
    {
        public IEnumerable<Quest> Quests => _quests;
    
        [SerializeField] private List<Quest> _quests;
    }
}