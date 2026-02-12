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
                            questDataProxy.IsActiveDestination.Value = 
                                !quest.Missions[questDataProxy.CurrentMission.Value].DestinationParent.Destination.NotHighlightOnAwake;
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
                Debug.LogError("No questID here");
                return;
            }

            var quest = _questsDict[questID];
            
            var questDataProxy = new QuestDataProxy(new QuestData
            {
                Id = quest.Id,
                CurrentMission = 0,
                IsActiveDestination = !quest.Missions[0].DestinationParent.Destination.NotHighlightOnAwake
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