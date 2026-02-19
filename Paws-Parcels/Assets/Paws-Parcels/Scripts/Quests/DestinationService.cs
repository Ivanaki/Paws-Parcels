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
    public class DestinationService
    {
        //словарь имя-destination
        private readonly Dictionary<string, MarkObject> _allMarksDict = new();
        
        private readonly Dictionary<string, CompositeDisposable> _questSubscriptionsDict = new();
        private readonly Dictionary<string, List<string>> _allCurrentShowableMarksDict = new();

        private string _lastSelectedQuest = null;
        
        public DestinationService(GameStateProvider gameStateProvider)
        {
            var activeQuests = gameStateProvider.GameProxy.ActiveQuests;
            var selectedQuest = gameStateProvider.GameProxy.SelectedQuest;

            foreach (var qdp in activeQuests)
            {
                ActivateQuestMarkables(qdp);
            }
            
            activeQuests.ObserveAdd().Subscribe(e =>
                ActivateQuestMarkables(e.Value));
            
            activeQuests.ObserveRemove().Subscribe(e =>
            {
                var qdp = e.Value;
                _questSubscriptionsDict[qdp.Id].Dispose();
                qdp.ActiveDestinations.ForEach(x => HideMarkableObject(qdp.Id, x));
            });
            
            activeQuests.ObserveClear().Subscribe(e =>
            {
                foreach (var value in _questSubscriptionsDict.Values) value.Dispose(); 
                _questSubscriptionsDict.Clear();

                HideAll();
            });

            selectedQuest.Subscribe(newQuest =>
            {
                if (string.IsNullOrEmpty(_lastSelectedQuest) == false)
                {
                    var qdpUnselect = activeQuests.FirstOrDefault(x => x.Id == _lastSelectedQuest);
                    if (qdpUnselect != null)
                        qdpUnselect.ActiveDestinations.ForEach(UnSelectMark);
                }
                
                _lastSelectedQuest = newQuest;
                
                var qdpSelect = activeQuests.FirstOrDefault(x => x.Id == newQuest);
                if (qdpSelect != null)
                {
                    qdpSelect.ActiveDestinations.ForEach(SelectMark);
                }
            });
            
            return;
            
            void ActivateQuestMarkables(QuestDataProxy qdp)
            {
                var questId = qdp.Id;
                var disposable = new CompositeDisposable();
                
                qdp.ActiveDestinations.ForEach(x =>
                {
                    ShowMarkableObject(questId, x);
                    
                    if (_lastSelectedQuest == questId)
                        SelectMark(x);
                    else
                        UnSelectMark(x);
                });
                disposable.Add(qdp.ActiveDestinations.ObserveAdd().Subscribe(value =>
                {
                    ShowMarkableObject(questId, value.Value);
                    
                    if (_lastSelectedQuest == questId)
                        SelectMark(value.Value);
                    else
                        UnSelectMark(value.Value);
                }));
                disposable.Add(qdp.ActiveDestinations.ObserveRemove().Subscribe(value => 
                    HideMarkableObject(questId, value.Value)));
                disposable.Add(qdp.ActiveDestinations.ObserveClear().Subscribe(_ => 
                    HideAllQuestMarkables(questId)));

                _questSubscriptionsDict[questId] = disposable;
            }
        }
        
        
        private void ShowMarkableObject(string questId, string destinationId)
        {
            //добавляем количество квестов, которые подсвечивают один и тот же объект
            if (_allCurrentShowableMarksDict.TryAdd(destinationId, new List<string>(){questId}) == false)
                _allCurrentShowableMarksDict[destinationId].Add(questId);
            
            if (_allMarksDict.TryGetValue(destinationId, out var markObject))
            {
                markObject.Show();
            }
        }
        
        private void HideMarkableObject(string questID, string destinationId)
        {
            if (_allCurrentShowableMarksDict.ContainsKey(destinationId))
            {
                _allCurrentShowableMarksDict[destinationId].Remove(questID);
                
                if (_allCurrentShowableMarksDict[destinationId].Count == 0)
                {
                    _allCurrentShowableMarksDict.Remove(destinationId);
                    if (_allMarksDict.TryGetValue(destinationId, out var markObject))
                    {
                        markObject.Hide();
                    }
                }
            }
        }

        private void HideAllQuestMarkables(string questID)
        {
            var destinations = new List<string>();
            
            foreach (var keyValuePair in _allCurrentShowableMarksDict)
            {
                if (keyValuePair.Value.Contains(questID))
                {
                    //remove
                    destinations.Add(keyValuePair.Key);
                }
            }
            
            destinations.ForEach(x => HideMarkableObject(questID, x));
        }

        private void HideAll()
        {
            foreach (var value in _allCurrentShowableMarksDict.Keys)
            {
                if (_allMarksDict.TryGetValue(value, out var markObject))
                {
                    markObject.Hide();
                }
            }
            _allCurrentShowableMarksDict.Clear();
        }

        private void SelectMark(string destinationId)
        {
            if (_allMarksDict.TryGetValue(destinationId, out var markObject))
            {
                markObject.Select();
            }
        }

        private void UnSelectMark(string destinationId)
        {
            if (_allMarksDict.TryGetValue(destinationId, out var markObject))
            {
                markObject.Unselect();
            }
        }
        
        public void AddMark(MarkObject markObject)
        {
            var result = _allMarksDict.TryAdd(markObject.Id, markObject);
            
            if (result)
            {
                if (_allCurrentShowableMarksDict.ContainsKey(markObject.Id))
                {
                    markObject.Show();
                    
                    if (_lastSelectedQuest == _allCurrentShowableMarksDict[markObject.Id].FirstOrDefault(x => x == _lastSelectedQuest))
                        markObject.Select();
                    else
                        markObject.Unselect();
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
            markObject.Hide();
        }
    }
}