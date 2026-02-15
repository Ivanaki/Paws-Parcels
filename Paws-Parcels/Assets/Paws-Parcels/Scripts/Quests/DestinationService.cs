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
        
        
        private readonly Dictionary<string, CompositeDisposable> _questSubscriptionsDict = new();
        //private readonly Dictionary<string, CompositeDisposable> _onOffHighLightDisposablesDict = new();
        //private readonly HashSet<string> _allCurrentShowableMarksSet = new();
        private readonly Dictionary<string, List<string>> _allCurrentShowableMarksDict = new();
        //private readonly Dictionary<string, string> _questDict = new();
        //private readonly Dictionary<string, HashSet<string>> _questSortedMarksDict = new();
        //private readonly HashSet<string> _selectedMarksSet = new();

        private string _lastSelectedQuest = null;
        
        public DestinationService(GameStateProvider gameStateProvider/*, QuestsDatabase questsDatabase*/)
        {
            //var allQuestsDict = questsDatabase.QuestsDict;
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
                /*qdp.IsActiveDestination.Value = false;
                _questSubscriptionsDict[qdp.Id].Dispose();
                _onOffHighLightDisposablesDict[qdp.Id].Dispose();*/
            });
            
            activeQuests.ObserveClear().Subscribe(e =>
            {
                foreach (var value in _questSubscriptionsDict.Values) value.Dispose(); 
                _questSubscriptionsDict.Clear();
                
                /*foreach (var value in _onOffHighLightDisposablesDict.Values) value.Dispose();
                _onOffHighLightDisposablesDict.Clear();*/

                HideAll();
            });

            selectedQuest.Subscribe(newQuest =>
            {
                if (string.IsNullOrEmpty(_lastSelectedQuest) == false)
                {
                    var qdpUnselect = activeQuests.FirstOrDefault(x => x.Id == _lastSelectedQuest);
                    if (qdpUnselect != null)
                        qdpUnselect.ActiveDestinations.ForEach(UnSelectMark);
                    /*foreach (var mark in _selectedMarksSet) UnSelectMark(mark);
                    _selectedMarksSet.Clear();*/
                }
                
                _lastSelectedQuest = newQuest;
                
                var qdpSelect = activeQuests.FirstOrDefault(x => x.Id == newQuest);
                if (qdpSelect != null)
                {
                    qdpSelect.ActiveDestinations.ForEach(s =>
                    {
                        SelectMark(s);
                        //_selectedMarksSet.Add(s);
                    });
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
                /*var quest = allQuestsDict[qdp.Id];

                var dis2 = qdp.IsActiveDestination.Subscribe(UpdateMarkable);

                //CompositeDisposable disposable = null;
                var dis1 = qdp.CurrentMission.Subscribe(v =>
                {
                    if (_onOffHighLightDisposablesDict.TryGetValue(qdp.Id, out var compositeDisposable))
                        compositeDisposable?.Dispose();

                    var disposable = new CompositeDisposable();
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

                    //когда меняется миссия нужно деактивировать все прошлые подсказки (кроме 0 миссии)
                    if(v > 0)
                        HideMarkableObject(quest.Missions[v-1].DestinationParent.Destination.Id);

                    //активировать все новый подсказки, кроме тех, которые отметили, что их не нужно в SO
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
                }*/
            }
        }
        
        
        private void ShowMarkableObject(string questId, string destinationId)
        {
            //добавляем количество квестов, которые подсвечивают один и тот же объект
            if (_allCurrentShowableMarksDict.TryAdd(destinationId, new List<string>(){questId}) == false)
                _allCurrentShowableMarksDict[destinationId].Add(questId);
            
            /*if (_questSortedMarksDict.ContainsKey(questID) == false)
                _questSortedMarksDict.Add(questID, new HashSet<string>());

            _questSortedMarksDict[questID].Add(destinationId);
            _allCurrentShowableMarksSet.Add(destinationId);*/
            
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
            
            /*if(_questSortedMarksDict.ContainsKey(questID))
                _questSortedMarksDict.Remove()
            _allCurrentShowableMarksSet.Remove(destinationId);
                
            if (_allMarksDict.TryGetValue(id, out var markObject))
            {
                markObject.Hide();
            }*/
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
        }
    }
}