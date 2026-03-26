using System.Collections.Generic;
using Paws_Parcels.NPCDialogs;
using UnityEngine;

namespace Dialogue
{
    public class DialogueInQuestsService
    {
        private readonly Dictionary<string, NPC> _npcOnSceneDict = new();
        
        public void RegisterNpc(NPC npc)
        {
            var addResult = _npcOnSceneDict.TryAdd(npc.NpcName, npc);
            
            if (addResult == false)
            {
                Debug.LogError($"Failed to register npc {npc.NpcName}. Npc already registered.");
            }
        }

        public void UnregisterNpc(NPC npc)
        {
            var deleteResult = _npcOnSceneDict.Remove(npc.NpcName);

            if (deleteResult == false)
            {
                Debug.LogError($"Failed to unregister npc {npc.NpcName}. Npc not registered.");
            }
        }
    }
}