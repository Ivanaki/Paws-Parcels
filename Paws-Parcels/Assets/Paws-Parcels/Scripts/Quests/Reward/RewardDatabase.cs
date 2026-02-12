using System.Collections.Generic;
using UnityEngine;

namespace Paws_Parcels.Scripts.Quests
{
    [CreateAssetMenu(fileName = "RewardDatabase", menuName = "Paws_parcels/RewardDatabase")]
    public class RewardDatabase : ScriptableObject
    {
        public IEnumerable<Reward> Rewards => _rewards;
    
        [SerializeField] private List<Reward> _rewards;
    }
}