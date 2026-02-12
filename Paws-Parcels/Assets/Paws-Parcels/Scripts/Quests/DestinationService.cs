using System;
using System.Collections.Generic;
using System.Linq;
using ObservableCollections;
using Paws_Parcels.Scripts.Quests;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Quests.Message;
using R3;
using Services;
using UnityEngine;
using Zenject;

namespace Quests
{
    public class DestinationService/* : IInitializable*/
    {
        //private readonly MessageSystem _messageSystem;
        //private const float BaseShowableDistance = 5f;
        
        //словарь имя-destination
        private readonly Dictionary<string, MarkObject> _allMarksDict = new();
        
        
        private readonly Dictionary<string, IDisposable> _questSubscriptionsDict = new();
        private readonly Dictionary<string, CompositeDisposable> _onOffHighLightDisposablesDict = new();
        private readonly HashSet<string> _currentShowableMarksSet = new();
        private readonly HashSet<string> _selectedMarksSet = new();
        
        
        public DestinationService(GameStateProvider gameStateProvider, MessageSystem messageSystem,
            QuestsDatabase questsDatabase)
        {
            //_messageSystem = messageSystem;
            var allQuestsDict = questsDatabase.QuestsDict;
            var activeQuests = gameStateProvider.GameProxy.ActiveQuests;
            var selectedQuest = gameStateProvider.GameProxy.SelectedQuest;

            foreach (var qdp in activeQuests)
            {
                ActivateMarkable(qdp);
            }
            
            activeQuests.ObserveAdd().Subscribe(e =>
            {
                var qdp = e.Value;
                ActivateMarkable(qdp);
            });
            
            activeQuests.ObserveRemove().Subscribe(e =>
            {
                var qdp = e.Value;
                qdp.IsActiveDestination.Value = false;
                _questSubscriptionsDict[qdp.Id].Dispose();
                _onOffHighLightDisposablesDict[qdp.Id].Dispose();
            });
            
            activeQuests.ObserveClear().Subscribe(e =>
            {
                foreach (var value in _questSubscriptionsDict.Values) value.Dispose(); 
                _questSubscriptionsDict.Clear();
                
                foreach (var value in _onOffHighLightDisposablesDict.Values) value.Dispose();
                _onOffHighLightDisposablesDict.Clear();

                foreach (var markObject in _allMarksDict.Values)
                {
                    HideMarkableObject(markObject.Id);
                }
            });

            /*selectedQuest.Subscribe(v =>
            {
                _selectedMarksSet.Clear();
                var quest = allQuestsDict[v].Missions[activeQuests.FirstOrDefault(x => x.Id == v).CurrentMission.Value];
                
            });*/
            
            return;
            
            void ActivateMarkable(QuestDataProxy qdp)
            {
                var quest = allQuestsDict[qdp.Id];

                CompositeDisposable disposable = null;

                var dis2 = qdp.IsActiveDestination.Subscribe(UpdateMarkable);
                
                var dis1 = qdp.CurrentMission.Subscribe(v =>
                {
                    disposable?.Dispose();
                    disposable = new CompositeDisposable();
                    _onOffHighLightDisposablesDict[qdp.Id] = disposable;

                    var offHighlightMessage = quest.Missions[v].DestinationParent.Destination.OffHighlightMessage;
                    disposable.Add(messageSystem.AddListener(offHighlightMessage).Subscribe(args =>
                    {
                        qdp.IsActiveDestination.Value = false;
                    }));
                    
                    var onHighlightMessage = quest.Missions[v].DestinationParent.Destination.OnHighlightMessage;
                    disposable.Add(messageSystem.AddListener(onHighlightMessage).Subscribe(args =>
                    {
                        qdp.IsActiveDestination.Value = true;
                    }));
                    
                    
                    if(v > 0)
                        HideMarkableObject(quest.Missions[v-1].DestinationParent.Destination.Id);
                    
                    UpdateMarkable(qdp.IsActiveDestination.Value);
                });

                

                _questSubscriptionsDict[qdp.Id] = Disposable.Combine(dis1, dis2);
                
                return;
                
                void UpdateMarkable(bool v)
                {
                    if (v)
                    {
                        ShowMarkableObject(quest.Missions[qdp.CurrentMission.Value].DestinationParent.Destination.Id);
                    }
                    else
                    {
                        HideMarkableObject(quest.Missions[qdp.CurrentMission.Value].DestinationParent.Destination.Id);
                    }
                }
            }
        }

        
        
        
        private void ShowMarkableObject(string id)
        {
            _currentShowableMarksSet.Add(id);
            
            if (_allMarksDict.TryGetValue(id, out var markObject))
            {
                markObject.Show();
            }
        }
        
        private void HideMarkableObject(string id)
        {
            _currentShowableMarksSet.Remove(id);
                
            if (_allMarksDict.TryGetValue(id, out var markObject))
            {
                markObject.Hide();
            }
        }
        
        public void AddMark(MarkObject markObject)
        {
            var result = _allMarksDict.TryAdd(markObject.Id, markObject);
            
            if (result)
            {
                if (_currentShowableMarksSet.Contains(markObject.Id))
                {
                    markObject.Show();
                }
            }
            else
            {
                Debug.LogError($"Marks object has same id in scene: {markObject.Id}");
            }
        }

        public void RemoveMark(MarkObject markObject)
        {
            _allMarksDict.Remove(markObject.Id);
        }
    }
}