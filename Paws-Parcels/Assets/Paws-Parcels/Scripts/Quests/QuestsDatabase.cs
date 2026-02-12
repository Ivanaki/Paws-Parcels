using System;
using System.Collections.Generic;
using Paws_Parcels.Scripts.Quests;
using UnityEngine;

namespace Quests
{
    public class QuestsDatabase
    {
        public IReadOnlyDictionary<string, Quest> QuestsDict => _questsDict;
        
        private readonly Dictionary<string, Quest> _questsDict = new();
        
        public QuestsDatabase()
        {
            var database = Resources.Load<QuestsDatabaseScriptableObject>("Editor/Quests/QuestsDatabase");
            if (database == null) throw new NullReferenceException("No QuestsDatabase for path Editor/Quests/QuestsDatabase");
            
            foreach (var quest in database.Quests)
            {
                if (quest == null)
                {
                    continue;
                }
                
                if (quest.Missions.Count == 0)
                {
                    Debug.LogError($"quest {quest.Id} has no missions in it");
                    continue;
                }

                var result = _questsDict.TryAdd(quest.Id, quest);
                
                if (result == false)
                { 
                    Debug.LogError("Same id in 2 quests. Need fix it. This quest dont added with id: " + quest.Id);
                }
            }
        }
    }
}