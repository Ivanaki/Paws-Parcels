using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ObservableCollections;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Quests;
using Quests.Message;
using R3;
using Services;
using UnityEngine;

namespace Paws_Parcels.Scripts.Quests
{
    //список всех messages:
    //ActivateQuest:[questId] - это сообщение активирует заданный квест в параметре
    //
    
    //список всего текста квесте:
    //[questId]:Name
    //[questId]:[missionId]:Title
    //[questId]:[missionId]:Description
    
    public class QuestsService : IDisposable
    {
        //public IObservableCollection<QuestData> ActiveQuests => _activeQuests;
        

        private readonly RewardService _rewardService;
        private readonly MessageSystem _messageSystem;
        //private readonly DestinationService _destinationService;

        private IReadOnlyDictionary<string, Quest> _questsDict { get; }

        private ObservableList<QuestDataProxy> ActiveQuests { get; }
        private ObservableList<string> SucceedQuests { get; }
        private ObservableList<string> CurrentActiveDestinations { get; }
        private ReactiveProperty<string> SelectedQuest { get; }
        
        private CompositeDisposable _compositeDisposable = new();

        private readonly Dictionary<string, CompositeDisposable> _questId_ReactiveMessage_Dict = new();
        
        private readonly Dictionary<string, CompositeDisposable> _onOffHighLightDisposablesDict = new();
        private readonly Dictionary<string, IDisposable> _questSubscriptionsDict = new();
        
        public QuestsService(RewardService rewardService, MessageSystem messageSystem, GameStateProvider gameStateProvider,
            QuestsDatabase questsDatabase)
        {
            _questsDict = questsDatabase.QuestsDict;
            ActiveQuests = gameStateProvider.GameProxy.ActiveQuests;
            SucceedQuests = gameStateProvider.GameProxy.SucceedQuests;
            CurrentActiveDestinations = gameStateProvider.GameProxy.CurrentActiveDestinations;
            SelectedQuest = gameStateProvider.GameProxy.SelectedQuest;
            
            _rewardService = rewardService;
            _messageSystem = messageSystem;
            
            ActiveQuests.ForEach(NewQuestAdded);

            ActiveQuests.ObserveAdd().Subscribe(e => NewQuestAdded(e.Value));

            ActiveQuests.ObserveRemove().Subscribe(e => QuestRemove(e.Value));
            
            ActiveQuests.ObserveClear().Subscribe(e => ClearQuests());
            
            foreach (var quest in _questsDict.Values)
            {
                _messageSystem.AddListener($"ActivateQuest:{quest.Id}").Subscribe(_ =>
                {
                    ActiveNewQuest(quest.Id);
                    /*if (_questsDict.TryGetValue(quest.Id, out var quest))
                    {
                        
                    }
                    else
                    {
                        Debug.LogError("Quest not find while try activate this quest: " + messageArgs.message + ":" + messageArgs.parameter);
                    }*/
                });
            }
        }

        private void NewQuestAdded(QuestDataProxy questDataProxy)
        {
            //var questDataProxy = e.Value;

            var questId = questDataProxy.Id;
            var quest = _questsDict[questId];
            var missionsLenght = quest.Missions.Count;

            //var rm = new ReactiveMessage();
            //_questId_ReactiveMessage_Dict[questId] = rm;

            SubscribeMissionComplete(questDataProxy.CurrentMission.Value);

            //добавить destinations
#region Destinations

            var disposable1 = questDataProxy.CurrentMission.Subscribe(v =>
            {
                if (_onOffHighLightDisposablesDict.TryGetValue(questDataProxy.Id, out var compositeDisposable))
                    compositeDisposable?.Dispose();

                var disposable = new CompositeDisposable();
                _onOffHighLightDisposablesDict[questDataProxy.Id] = disposable;

                var destination = quest.Missions[v].DestinationParent.Destination;
                var destinations = quest.Missions[v].DestinationParent.ManyDestinations.Destinations;

                if (quest.Missions[v].HasDestination)
                {
                    if (quest.Missions[v].DestinationParent.IsMany == false)
                    {
                        Subscribe(destination);
                    }
                    else
                    {
                        destinations.ForEach(Subscribe);
                    }
                }

                void Subscribe(DestinationAsset destinationAsset)
                {
                    disposable.Add(_messageSystem.AddListenerWithManyMessages(destinationAsset.OffHighlightMessage).Subscribe(args =>
                    {
                        //remove
                        questDataProxy.ActiveDestinations.Remove(destinationAsset.Id);
                    }));

                    disposable.Add(_messageSystem.AddListenerWithManyMessages(destinationAsset.OnHighlightMessage).Subscribe(args =>
                    {
                        //add
                        if (questDataProxy.ActiveDestinations.Contains(destinationAsset.Id) == false)
                            questDataProxy.ActiveDestinations.Add(destinationAsset.Id);
                    }));
                }
            });


            //отдельно, тк не нужно очищать ActiveDestinations при инициализации, но нужно при увеличении миссии
            var disposable2 = questDataProxy.CurrentMission.Skip(1).Subscribe(v =>
            {
                var destination = quest.Missions[v].DestinationParent.Destination;
                var destinations = quest.Missions[v].DestinationParent.ManyDestinations.Destinations;

                //убрать все destinations прошлой миссии
                questDataProxy.ActiveDestinations.Clear();

                //активировать все новые, кроме тех, которые отметили, что их не нужно в SO
                if (quest.Missions[v].HasDestination)
                {
                    if (quest.Missions[v].DestinationParent.IsMany == false)
                    {
                        Active(destination);
                    }
                    else
                    {
                        destinations.ForEach(Active);
                    }
                }

                void Active(DestinationAsset destinationAsset)
                {
                    if (destinationAsset.NotHighlightOnAwake == false)
                    {
                        questDataProxy.ActiveDestinations.Add(destinationAsset.Id);
                    }
                }
            });
            
            _questSubscriptionsDict[questDataProxy.Id] = Disposable.Combine(disposable1, disposable2);

#endregion
            return;
            
            void SubscribeMissionComplete(int currentMission)
            {
                if(_questId_ReactiveMessage_Dict.TryGetValue(questId, out var disposable)) disposable.Dispose();
                
                disposable = new CompositeDisposable();
                _questId_ReactiveMessage_Dict[questId] = disposable;
                
                var mission = quest.Missions[currentMission];
                
                disposable.Add(_messageSystem.AddListenerWithManyMessages(mission.Message.Message).Subscribe(_ =>
                {
                    questDataProxy.MessageProgress.Value++;
                }));
                disposable.Add(_messageSystem.AddListenerWithManyMessages(mission.Message.MinusMessage).Subscribe(_ =>
                {
                    questDataProxy.MessageProgress.Value = Mathf.Max(0, questDataProxy.MessageProgress.Value-1);
                }));
                
                disposable.Add(questDataProxy.MessageProgress.Subscribe(v =>
                {
                    if ((v >= 1 && mission.Message.HasCount == false) || (v >= mission.Message.MessageCount && mission.Message.HasCount))
                    {
                        disposable.Dispose();
                    
                        if (questDataProxy.CurrentMission.Value + 1 < missionsLenght)
                        {
                            //подписка на следующую миссию
                            questDataProxy.MessageProgress.Value = 0;
                            questDataProxy.CurrentMission.Value++;
                        
                            //questDataProxy.IsActiveDestination.Value = 
                            //    !quest.Missions[questDataProxy.CurrentMission.Value].DestinationParent.Destination.NotHighlightOnAwake;
                            SubscribeMissionComplete(currentMission + 1);
                        }
                        else
                        {
                            //квест выполнен
                            CompleteQuest(questId);
                        }
                    }
                }));
            }
        }

        private void QuestRemove(QuestDataProxy qdp)
        {
            if (_onOffHighLightDisposablesDict.TryGetValue(qdp.Id, out var disposable1))
            {
                disposable1.Dispose();
                _onOffHighLightDisposablesDict.Remove(qdp.Id);
            }
                
            if (_questSubscriptionsDict.TryGetValue(qdp.Id, out var disposable2))
            {
                disposable2.Dispose();
                _questSubscriptionsDict.Remove(qdp.Id);
            }
                
            if(_questId_ReactiveMessage_Dict.TryGetValue(qdp.Id, out var reactiveMessage))
            {
                reactiveMessage.Dispose();
                _questId_ReactiveMessage_Dict.Remove(qdp.Id);
            }
        }

        private void ClearQuests()
        {
            foreach (var dis in _onOffHighLightDisposablesDict.Values) dis.Dispose();
            _onOffHighLightDisposablesDict.Clear();
            
            foreach (var dis in _questSubscriptionsDict.Values) dis.Dispose();
            _questSubscriptionsDict.Clear();
            
            foreach (var dis in _questId_ReactiveMessage_Dict.Values) dis.Dispose();
            _questId_ReactiveMessage_Dict.Clear();
        }

        public void ActiveNewQuest(string questID)
        {
            if (_questsDict.ContainsKey(questID) == false)
            {
                Debug.LogError("No questID here: " + questID);
                return;
            }

            if (ActiveQuests.FirstOrDefault(x => x.Id == questID) != null)
            {
                Debug.LogError("This quest is already active: " + questID);
                return;
            }

            if (SucceedQuests.Contains(questID))
            {
                Debug.LogError("This quest is succeed: " + questID);
                return;
            }
            
            var quest = _questsDict[questID];

            var destinations = new List<string>();
            var mission = quest.Missions[0];
            if (mission.HasDestination)
                if (mission.DestinationParent.IsMany == false)
                    AddDestination(mission.DestinationParent.Destination);
                else
                    mission.DestinationParent.ManyDestinations.Destinations.ForEach(AddDestination);
            
            var questDataProxy = new QuestDataProxy(new QuestData
            {
                Id = quest.Id,
                CurrentMission = 0,
                ActiveDestinations = destinations,
                MessageProgress = 0
            });
            
            ActiveQuests.Add(questDataProxy);


            void AddDestination(DestinationAsset destinationAsset)
            {
                if(destinationAsset.NotHighlightOnAwake == false)
                    destinations.Add(destinationAsset.Id);
            }
            //подписаться на сообщение о его завершении и тд пройтись по всем связям квестов с игрой
        }


        public void CompleteQuest(string questId)
        {
            var remove = ActiveQuests.FirstOrDefault(x => x.Id == questId);
            if (remove != null)
            {
                ActiveQuests.Remove(remove);
                SucceedQuests.Add(questId);
            }
            else
            {
                Debug.LogError($"quest {questId} is not active to be complete");
            }
        }
        
        public string GetQuestCode(int questId)
        {
            return "testMissionName";
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }
    }
}