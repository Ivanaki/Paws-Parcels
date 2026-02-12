using System;
using System.Collections.Generic;
using Quests;
using UnityEngine;

namespace Paws_Parcels.Scripts.Quests
{
    public class RewardService
    {
        public IReadOnlyDictionary<string, Reward> RewardsDict => _rewardsDict;
        private Dictionary<string, Reward> _rewardsDict = new();
        
        public RewardService()
        {
            var database = Resources.Load<RewardDatabase>("Editor/Rewards/RewardDatabase");
            if (database == null) throw new NullReferenceException("No RewardDatabase for path Editor/Rewards/RewardDatabase");
            
            foreach (var reward in database.Rewards)
            {
                var resultAdd = _rewardsDict.TryAdd(reward.Id, reward);
                if (resultAdd == false)
                {
                    Debug.LogError("Same id in 2 or more rewards. Need fix it. This reward dont added.");
                }
            }
        }
    }
}