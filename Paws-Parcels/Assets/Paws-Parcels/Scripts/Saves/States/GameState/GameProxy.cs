using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using ObservableCollections;
using Paws_Parcels.Scripts.Quests;
using Paws_Parcels.Scripts.Saves.States.GameResources;
using R3;
using UnityEngine;

namespace Paws_Parcels.Scripts.Saves.States.GameState
{
    public class GameProxy
    {
        public readonly GameState Origin;
        
        public Guid SaveId => Origin.SaveId;
        public ReactiveProperty<int> TrackedMissionID { get; }
        public ReactiveProperty<bool> IsCompassActive { get; }
        public ReadOnlyReactiveProperty<string> SelectedQuest => _selectedQuest;
        public ReadOnlyReactiveProperty<bool> HasMissionTimer => _hasMissionTimer;
        public ReadOnlyReactiveProperty<float> MissionTimerSeconds => _missionTimerSeconds;
        public ReadOnlyReactiveProperty<float> MissionTimerTotalSeconds => _missionTimerTotalSeconds;
        
        private ReactiveProperty<string> _selectedQuest;
        private readonly ReactiveProperty<bool> _hasMissionTimer;
        private readonly ReactiveProperty<float> _missionTimerSeconds;
        private readonly ReactiveProperty<float> _missionTimerTotalSeconds;
        private readonly Dictionary<QuestDataProxy, CompositeDisposable> _questTimerSubscriptions = new();

        public ObservableList<QuestDataProxy> ActiveQuests { get; } = new();
        public ObservableList<string> SucceedQuests { get; } = new();
        public ObservableList<string> CurrentActiveDestinations { get; } = new();

        public PlayerSaveData PlayerData
        {
            get => Origin.PlayerData;
            set => Origin.PlayerData = value;
        }
        
        public CarSaveData CarData
        {
            get => Origin.CarData;
            set => Origin.CarData = value;
        }
        
        public ObservableList<ItemProxy> ItemsInWorld { get; } = new();
        public ReactiveProperty<bool> IsItemInHandsNull { get; }
        public ReactiveProperty<string> InHandsItemTypeId { get; }
        public string InHandsDestinationId { get => Origin.InHandsDestinationId; set => Origin.InHandsDestinationId = value; }
        
        public ObservableList<ItemClearProxy> ItemsInsideCar { get; } = new();
        
        public ReactiveProperty<bool> IsPlayerInCar { get; }
        public ReactiveProperty<bool> IsCarDoorOpen { get; }
        
        public ObservableList<ResourceProxy> Resources { get; } = new();
        
        public ObservableDictionary<string, ItemDestinationProxy> ItemsDestinationsPutters { get; } = new();
        
        public ReactiveProperty<bool> IsCatInCar { get; }
        
        public ReactiveProperty<bool> StopCar { get; }

        public GameProxy(GameState origin)
        {
            //не забудь изменить change внизу
            Origin = origin;
            
            TrackedMissionID = new ReactiveProperty<int>(origin.TrackedMissionID);
            IsCompassActive = new ReactiveProperty<bool>(origin.IsCompassActive);
            _selectedQuest = new ReactiveProperty<string>(origin.SelectedQuest);
            _hasMissionTimer = new ReactiveProperty<bool>(false);
            _missionTimerSeconds = new ReactiveProperty<float>(0f);
            _missionTimerTotalSeconds = new ReactiveProperty<float>(0f);
            
            TrackedMissionID.Skip(1).Subscribe(v => origin.TrackedMissionID = v);
            IsCompassActive.Skip(1).Subscribe(v => origin.IsCompassActive = v);
            _selectedQuest.Skip(1).Subscribe(v => origin.SelectedQuest = v);
            
            InitQuests(origin);
            InitMissionTimerAggregation();
            
            InitItems(origin);

            IsItemInHandsNull = new ReactiveProperty<bool>(origin.IsItemInHandsNull);
            IsItemInHandsNull.Skip(1).Subscribe(v => origin.IsItemInHandsNull = v);
            
            InHandsItemTypeId = new ReactiveProperty<string>(origin.InHandsItemTypeId);
            InHandsItemTypeId.Skip(1).Subscribe(v => origin.InHandsItemTypeId = v);

            IsPlayerInCar = new ReactiveProperty<bool>(origin.IsPlayerInCar);
            IsPlayerInCar.Skip(1).Subscribe(v => origin.IsPlayerInCar = v);

            IsCarDoorOpen = new ReactiveProperty<bool>(origin.IsCarDoorOpen);
            IsCarDoorOpen.Skip(1).Subscribe(v => origin.IsCarDoorOpen = v);

            IsCatInCar = new ReactiveProperty<bool>(origin.IsCatInCar);
            IsCatInCar.Skip(1).Subscribe(v => origin.IsCatInCar = v);

            StopCar = new ReactiveProperty<bool>(origin.StopCar);
            StopCar.Skip(1).Subscribe(v => origin.StopCar = v);

            InitResources(origin);
            InitItemsDestinations(origin);
        }

        public void Change(GameState origin)
        {
            //обязательно копируем, если это class (ссылки могут остаться)
            
            TrackedMissionID.Value = origin.TrackedMissionID;
            IsCompassActive.Value = origin.IsCompassActive;
            _selectedQuest.Value = origin.SelectedQuest;
            
            ActiveQuests.Clear();
            origin.ActiveQuests.ForEach(questData => ActiveQuests.Add(new QuestDataProxy(new QuestData
            {
                Id = questData.Id,
                CurrentMission = questData.CurrentMission,
                ActiveDestinations = new List<string>(questData.ActiveDestinations),
                MessageProgress = questData.MessageProgress,
                HasMissionTimer = questData.HasMissionTimer,
                MissionTimerSeconds = questData.MissionTimerSeconds,
                MissionTimerTotalSeconds = questData.MissionTimerTotalSeconds
            })));
            
            SucceedQuests.Clear();
            origin.SucceedQuests.ForEach(questData => SucceedQuests.Add(questData));
            
            CurrentActiveDestinations.Clear();
            origin.CurrentActiveDestinations.ForEach(questData => CurrentActiveDestinations.Add(questData));
            
            PlayerData = origin.PlayerData;
            CarData = origin.CarData;
            
            ItemsInWorld.Clear();
            origin.ItemsInWorld.ForEach(questData => ItemsInWorld.Add(new ItemProxy(questData)));

            IsItemInHandsNull.Value = origin.IsItemInHandsNull;
            InHandsItemTypeId.Value = origin.InHandsItemTypeId;
            InHandsDestinationId = origin.InHandsDestinationId;
            
            ItemsInsideCar.Clear();
            origin.ItemsInsideCar.ForEach(questData => ItemsInsideCar.Add(new ItemClearProxy(questData)));

            IsPlayerInCar.Value = origin.IsPlayerInCar;
            IsCarDoorOpen.Value = origin.IsCarDoorOpen;
            IsCatInCar.Value = origin.IsCatInCar;
            StopCar.Value = origin.StopCar;
            
            Resources.Clear();
            origin.Resources.ForEach(questData => Resources.Add(new ResourceProxy(questData)));

            ItemsDestinationsPutters.Clear();
            foreach (var originItemsDestination in origin.ItemsDestinations)
                ItemsDestinationsPutters.Add(originItemsDestination.Key, new ItemDestinationProxy(originItemsDestination.Value));

            UpdateAggregateMissionTimer();
        }
        
        /*public void Change(GameProxy gameProxy)
        {
            TrackedMissionID.Value = gameProxy.TrackedMissionID.Value;
            IsCompassActive.Value = gameProxy.IsCompassActive.Value;
        }*/

        public bool TrySelectQuestInData(string questId)
        {
            if (string.IsNullOrEmpty(questId) == false)
            {
                if (ActiveQuests.Any(x => x.Id == questId))
                {
                    _selectedQuest.Value = questId;
                    return true;
                }
            }

            Debug.LogError($"Quest {questId} dont selected");
            return false;
        }
        
        public int GetNewSavableEntityID()
        {
            return Origin.GetNewSavableEntityID();
        }
        
        public int GetNewNonSavableEntityID()
        {
            return Origin.GetNewNonSavableEntityID();
        }
        
        private void InitQuests(GameState origin)
        {
            origin.ActiveQuests.ForEach(questData => ActiveQuests.Add(new QuestDataProxy(questData)));
            
            ActiveQuests.ObserveAdd().Subscribe(e =>
            {
                var addedResource = e.Value;
                origin.ActiveQuests.Add(addedResource.Origin);
            });
            
            ActiveQuests.ObserveRemove().Subscribe(e =>
            {
                var removedResource = e.Value;
                origin.ActiveQuests.Remove(removedResource.Origin);
            });
            
            ActiveQuests.ObserveClear().Subscribe(e =>
            {
                origin.ActiveQuests.Clear();
            });
            
            origin.SucceedQuests.ForEach(questData => SucceedQuests.Add(questData));
            
            SucceedQuests.ObserveAdd().Subscribe(e =>
            {
                var addedResource = e.Value;
                origin.SucceedQuests.Add(addedResource);
            });
            
            SucceedQuests.ObserveRemove().Subscribe(e =>
            {
                var removedResource = e.Value;
                origin.SucceedQuests.Remove(removedResource);
            });
            
            SucceedQuests.ObserveClear().Subscribe(e =>
            {
                origin.SucceedQuests.Clear();
            });
            
            origin.CurrentActiveDestinations.ForEach(questData => CurrentActiveDestinations.Add(questData));
            
            CurrentActiveDestinations.ObserveAdd().Subscribe(e =>
            {
                var addedResource = e.Value;
                origin.CurrentActiveDestinations.Add(addedResource);
            });
            
            CurrentActiveDestinations.ObserveRemove().Subscribe(e =>
            {
                var removedResource = e.Value;
                origin.CurrentActiveDestinations.Remove(removedResource);
            });
            
            CurrentActiveDestinations.ObserveClear().Subscribe(e =>
            {
                origin.CurrentActiveDestinations.Clear();
            });
        }

        private void InitItems(GameState origin)
        {
            origin.ItemsInWorld.ForEach(item => ItemsInWorld.Add(new ItemProxy(item)));
            
            ItemsInWorld.ObserveAdd().Subscribe(e => 
                origin.ItemsInWorld.Add(e.Value.Origin));
            
            ItemsInWorld.ObserveRemove().Subscribe(e =>
                origin.ItemsInWorld.Remove(e.Value.Origin));
            
            ItemsInWorld.ObserveClear().Subscribe(e =>
                origin.ItemsInWorld.Clear());
            
            
            origin.ItemsInsideCar.ForEach(item => ItemsInsideCar.Add(new ItemClearProxy(item)));
            
            ItemsInsideCar.ObserveAdd().Subscribe(e => 
                origin.ItemsInsideCar.Add(e.Value.Origin));
            
            ItemsInsideCar.ObserveRemove().Subscribe(e =>
                origin.ItemsInsideCar.Remove(e.Value.Origin));
            
            ItemsInsideCar.ObserveClear().Subscribe(e =>
                origin.ItemsInsideCar.Clear());
        }

        private void InitResources(GameState origin)
        {
            origin.Resources.ForEach(item => Resources.Add(new ResourceProxy(item)));
            
            Resources.ObserveAdd().Subscribe(e => 
                origin.Resources.Add(e.Value.Origin));
            
            Resources.ObserveRemove().Subscribe(e =>
                origin.Resources.Remove(e.Value.Origin));
            
            Resources.ObserveClear().Subscribe(e =>
                origin.Resources.Clear());
        }

        private void InitItemsDestinations(GameState origin)
        {
            foreach (var originItemsDestination in origin.ItemsDestinations)
                ItemsDestinationsPutters.Add(originItemsDestination.Key, new ItemDestinationProxy(originItemsDestination.Value));
            
            ItemsDestinationsPutters.ObserveAdd().Subscribe(e => 
                origin.ItemsDestinations.Add(e.Value.Key, e.Value.Value.Origin));
            
            ItemsDestinationsPutters.ObserveRemove().Subscribe(e =>
                origin.ItemsDestinations.Remove(e.Value.Key));
            
            ItemsDestinationsPutters.ObserveClear().Subscribe(e =>
                origin.ItemsDestinations.Clear());
        }

        private void InitMissionTimerAggregation()
        {
            foreach (var activeQuest in ActiveQuests)
            {
                SubscribeQuestTimer(activeQuest);
            }

            ActiveQuests.ObserveAdd().Subscribe(e =>
            {
                SubscribeQuestTimer(e.Value);
                UpdateAggregateMissionTimer();
            });

            ActiveQuests.ObserveRemove().Subscribe(e =>
            {
                UnsubscribeQuestTimer(e.Value);
                UpdateAggregateMissionTimer();
            });

            ActiveQuests.ObserveClear().Subscribe(_ =>
            {
                foreach (var timerSubscription in _questTimerSubscriptions.Values)
                {
                    timerSubscription.Dispose();
                }

                _questTimerSubscriptions.Clear();
                UpdateAggregateMissionTimer();
            });

            UpdateAggregateMissionTimer();
        }

        private void SubscribeQuestTimer(QuestDataProxy questDataProxy)
        {
            if (_questTimerSubscriptions.ContainsKey(questDataProxy))
            {
                return;
            }

            var disposable = new CompositeDisposable();
            disposable.Add(questDataProxy.HasMissionTimer.Subscribe(_ => UpdateAggregateMissionTimer()));
            disposable.Add(questDataProxy.MissionTimerSeconds.Subscribe(_ => UpdateAggregateMissionTimer()));
            disposable.Add(questDataProxy.MissionTimerTotalSeconds.Subscribe(_ => UpdateAggregateMissionTimer()));

            _questTimerSubscriptions.Add(questDataProxy, disposable);
        }

        private void UnsubscribeQuestTimer(QuestDataProxy questDataProxy)
        {
            if (_questTimerSubscriptions.TryGetValue(questDataProxy, out var disposable))
            {
                disposable.Dispose();
                _questTimerSubscriptions.Remove(questDataProxy);
            }
        }

        private void UpdateAggregateMissionTimer()
        {
            var hasMissionTimer = false;
            var minTimeLeft = float.MaxValue;
            var totalTime = 0f;

            foreach (var activeQuest in ActiveQuests)
            {
                if (activeQuest.HasMissionTimer.CurrentValue == false)
                {
                    continue;
                }

                hasMissionTimer = true;
                var timeLeft = Mathf.Max(0f, activeQuest.MissionTimerSeconds.CurrentValue);

                if (timeLeft < minTimeLeft)
                {
                    minTimeLeft = timeLeft;
                    totalTime = Mathf.Max(0f, activeQuest.MissionTimerTotalSeconds.CurrentValue);
                }
            }

            _hasMissionTimer.Value = hasMissionTimer;
            _missionTimerSeconds.Value = hasMissionTimer ? minTimeLeft : 0f;
            _missionTimerTotalSeconds.Value = hasMissionTimer ? totalTime : 0f;
        }
    }
}
