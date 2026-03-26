using System.Collections.Generic;
using System.Linq;
using Paws_Parcels.NPCDialogs;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using UnityEngine;
using Yarn.Unity;

namespace Dialogue
{
    public class DialogueInQuestsService
    {
        private readonly Dictionary<string, INPC> _npcOnSceneDict = new();

        private ReactiveProperty<string> _selectedQuest { get; }
        
        public DialogueInQuestsService(GameStateProvider gameStateProvider)
        {
            _selectedQuest = gameStateProvider.GameProxy.SelectedQuest;

            _selectedQuest.Subscribe(v =>
            {
                UpdateAllNpcs();
            });
        }
        
        public void RegisterNpc(INPC npc)
        {
            var addResult = _npcOnSceneDict.TryAdd(npc.NpcName, npc);
            
            if (addResult == false)
            {
                Debug.LogError($"Failed to register npc {npc.NpcName}. Npc already registered.");
                return;
            }
            
            UpdateNpc(npc.NpcName);
        }

        public void UnregisterNpc(INPC npc)
        {
            var deleteResult = _npcOnSceneDict.Remove(npc.NpcName);

            if (deleteResult == false)
            {
                Debug.LogError($"Failed to unregister npc {npc.NpcName}. Npc not registered.");
            }
        }

        private readonly Dictionary<string, NPCData> _npcName_QuestIds = new();
        
        public void NewMission(string npcName, OneNpc oneNpc)
        {
            RemoveQuest(oneNpc.QuestId);

            if (_npcName_QuestIds.ContainsKey(npcName) == false)
            {
                _npcName_QuestIds.Add(npcName, new NPCData(new List<OneNpc>(), null));
            }
            
            _npcName_QuestIds[npcName].OneNpcs.Add(oneNpc);
            _npcName_QuestIds[npcName].EndDialogueReference = oneNpc.EndDialogueReference;

            UpdateNpc(npcName);
        }

        public void RemoveQuest(string questId)
        {
            foreach (var npcNameQuestId in _npcName_QuestIds)
            {
                npcNameQuestId.Value.OneNpcs.RemoveAll(x => x.QuestId == questId);
            }

            UpdateAllNpcs();
        }

        private void UpdateAllNpcs()
        {
            foreach (var keyValuePair in _npcOnSceneDict)
            {
                UpdateNpc(keyValuePair.Key);
            }
        }

        private void UpdateNpc(string npcName)
        {
            if (_npcOnSceneDict.TryGetValue(npcName, out var npc))
            {
                if (_npcName_QuestIds.TryGetValue(npcName, out var npcData))
                {
                    if (npcData.OneNpcs.Count > 0)
                    {
                        var hasSelected =
                            npcData.OneNpcs.FirstOrDefault(x => x.QuestId == _selectedQuest.CurrentValue) ??
                            npcData.OneNpcs.First();
                        
                        npc.SetDialog(hasSelected.MainDialogueReference);
                    }
                    else
                    {
                        npc.SetDialog(npcData.EndDialogueReference);
                    }
                }
            }
        }
    }

    public class NPCData
    {
        public List<OneNpc> OneNpcs;
        public DialogueReference EndDialogueReference;

        public NPCData(List<OneNpc> oneNpcs, DialogueReference endDialogueReference)
        {
            OneNpcs = oneNpcs;
            EndDialogueReference = endDialogueReference;
        }
    }
    
    public class OneNpc
    {
        public string QuestId;
        public DialogueReference MainDialogueReference;
        public DialogueReference EndDialogueReference;

        public bool NeedDelete = false;

        public OneNpc(string questId, DialogueReference mainDialogueReference,
            DialogueReference endDialogueReference)
        {
            QuestId = questId;
            MainDialogueReference = mainDialogueReference;
            EndDialogueReference = endDialogueReference;
        }
    }
}