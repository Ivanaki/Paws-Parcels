using System;
using System.Collections.Generic;
using System.Linq;
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
    
    public class QuestsService : IMessageHandler, IDisposable
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

        private readonly Dictionary<string, ReactiveMessage> _questId_ReactiveMessage_Dict = new();
        
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
            //_destinationService = destinationService;

            ActiveQuests.ObserveAdd().Subscribe(e =>
            {
                var questDataProxy = e.Value;
                
                var questId = questDataProxy.Id;
                var quest = _questsDict[questId];
                var missionsLenght = quest.Missions.Count;
                
                var rm = new ReactiveMessage();
                _questId_ReactiveMessage_Dict[questId] = rm;
            
                SubscribeMissionComplete(questDataProxy.CurrentMission.Value);
                
                //добавить destinations
                var disposable1 = questDataProxy.CurrentMission.Subscribe(v =>
                {
                    if (_onOffHighLightDisposablesDict.TryGetValue(questDataProxy.Id, out var compositeDisposable))
                        compositeDisposable?.Dispose();

                    var disposable = new CompositeDisposable();
                    _onOffHighLightDisposablesDict[questDataProxy.Id] = disposable;

                    var destination = quest.Missions[v].DestinationParent.Destination;
                    
                    disposable.Add(messageSystem.AddListener(destination.OffHighlightMessage).Subscribe(args =>
                    {
                        //remove
                        questDataProxy.ActiveDestinations.Remove(destination.Id);
                    }));
                    
                    disposable.Add(messageSystem.AddListener(destination.OnHighlightMessage).Subscribe(args =>
                    {
                        //add
                        if(questDataProxy.ActiveDestinations.Contains(destination.Id) == false)
                            questDataProxy.ActiveDestinations.Add(destination.Id);
                    }));
                    
                    //TODO ошибка здесь логическая

                    /*
                    //когда меняется миссия нужно деактивировать все прошлые подсказки (кроме 0 миссии)
                    if(v > 0)
                        HideMarkableObject(quest.Missions[v-1].DestinationParent.Destination.Id);

                    //активировать все новый подсказки, кроме тех, которые отметили, что их не нужно в SO
                    UpdateMarkable(questDataProxy.IsActiveDestination.Value);*/
                });

                
                //отдельно, тк не нужно очищать ActiveDestinations при инициализации, но нужно при увеличении миссии
                var disposable2 = questDataProxy.CurrentMission.Skip(1).Subscribe(v =>
                {
                    var destination = quest.Missions[v].DestinationParent.Destination;
                    
                    //убрать все destinations прошлой миссии
                    questDataProxy.ActiveDestinations.Clear();

                    //активировать все новые, кроме тех, которые отметили, что их не нужно в SO
                    if (destination.NotHighlightOnAwake == false)
                    {
                        questDataProxy.ActiveDestinations.Add(destination.Id);
                    }
                });

                _questSubscriptionsDict[questDataProxy.Id] = Disposable.Combine(disposable1, disposable2);

                return;

                void SubscribeMissionComplete(int currentMission)
                {
                    var mission = quest.Missions[currentMission];
                    rm.Clear();
                
                    _messageSystem.AddListener(rm, mission.Message.Message);
                    /*if (mission.HasDestination)
                    {
                        _destinationService.ShowMarkableObject(mission.DestinationParent.Destination.DestinationId);
                    }*/
                    
                    
                    rm.Observe.Subscribe(v =>
                    {
                        _messageSystem.RemoveListener(rm, mission.Message.Message);
                        /*if (mission.HasDestination)
                        {
                            _destinationService.HideMarkableObject(mission.DestinationParent.Destination.DestinationId);
                        }*/

                        if (questDataProxy.CurrentMission.Value + 1 < missionsLenght)
                        {
                            //подписка на следующую миссию
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
                    });
                }
            });

            ActiveQuests.ObserveRemove().Subscribe(e =>
            {
                if (_onOffHighLightDisposablesDict.TryGetValue(e.Value.Id, out var disposable1))
                {
                    disposable1.Dispose();
                    _onOffHighLightDisposablesDict.Remove(e.Value.Id);
                }
                
                if (_questSubscriptionsDict.TryGetValue(e.Value.Id, out var disposable2))
                {
                    disposable2.Dispose();
                    _questSubscriptionsDict.Remove(e.Value.Id);
                }
                
                if(_questId_ReactiveMessage_Dict.TryGetValue(e.Value.Id, out var reactiveMessage))
                {
                    reactiveMessage.Dispose();
                    var quest = _questsDict[e.Value.Id];
                    var mission = quest.Missions[e.Value.CurrentMission.Value];
                    _messageSystem.RemoveListener(reactiveMessage, mission.Message.Message);
                    _questId_ReactiveMessage_Dict.Remove(e.Value.Id);
                }
            });
            
            ActiveQuests.ObserveClear().Subscribe(e =>
            {
                foreach (var pair in _questId_ReactiveMessage_Dict)
                {
                    var questId = pair.Key;
                    var reactiveMessage = pair.Value;

                    reactiveMessage.Dispose();

                    if (_questsDict.TryGetValue(questId, out var quest))
                    {
                        foreach (var valueMission in quest.Missions)
                        {
                            _messageSystem.RemoveListener(reactiveMessage, valueMission.Message.Message);
                        }
                    }
                }

                _questId_ReactiveMessage_Dict.Clear();
            });
            
            foreach (var quest in _questsDict.Values)
            {
                _messageSystem.AddListener(this, $"ActivateQuest:{quest.Id}");
            }
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
            
            var quest = _questsDict[questID];
            
            var questDataProxy = new QuestDataProxy(new QuestData
            {
                Id = quest.Id,
                CurrentMission = 0,
                //IsActiveDestination = !quest.Missions[0].DestinationParent.Destination.NotHighlightOnAwake
            });
            
            ActiveQuests.Add(questDataProxy);
            
            //подписаться на сообщение о его завершении и тд пройтись по всем связям квестов с игрой
        }


        public void CompleteQuest(string questId)
        {
            var remove = ActiveQuests.FirstOrDefault(x => x.Id == questId);
            if (remove != null)
            {
                ActiveQuests.Remove(remove);
                SucceedQuests.Add(questId);
                if (_questId_ReactiveMessage_Dict.TryGetValue(questId, out var reactiveMessage))
                {
                    reactiveMessage.Dispose();
                    _questId_ReactiveMessage_Dict.Remove(questId);
                }
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

        public void OnMessage(MessageArgs messageArgs)
        {
            if (messageArgs.message == "ActivateQuest")
            {
                if (_questsDict.TryGetValue(messageArgs.parameter, out var quest))
                {
                    ActiveNewQuest(quest.Id);
                }
                else
                {
                    Debug.LogError("Quest not find while try activate this quest: " + messageArgs.message + ":" + messageArgs.parameter);
                }
            }
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }
    }
}