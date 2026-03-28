using System;
using System.Collections.Generic;
using Items;
using ObservableCollections;
using Paws_Parcels.Scripts.Quests;
using Paws_Parcels.Scripts.Saves.States.GameResources;
using R3;

namespace Paws_Parcels.Scripts.Saves.States.GameState
{
    public class GameProxy
    {
        public readonly GameState Origin;
        
        public Guid SaveId => Origin.SaveId;
        public ReactiveProperty<int> TrackedMissionID { get; }
        public ReactiveProperty<bool> IsCompassActive { get; }
        public ReactiveProperty<string> SelectedQuest { get; }

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
        public string InHandsMarkId { get => Origin.InHandsMarkId; set => Origin.InHandsMarkId = value; }
        
        public ObservableList<ItemClearProxy> ItemsInsideCar { get; } = new();
        
        public ReactiveProperty<bool> IsPlayerInCar { get; }
        public ReactiveProperty<bool> IsCarDoorOpen { get; }
        
        public ObservableList<ResourceProxy> Resources { get; } = new();
        
        public ObservableDictionary<string, ItemDestinationProxy> ItemsDestinationsPutters { get; } = new();

        public GameProxy(GameState origin)
        {
            //не забудь изменить change внизу
            Origin = origin;
            
            TrackedMissionID = new ReactiveProperty<int>(origin.TrackedMissionID);
            IsCompassActive = new ReactiveProperty<bool>(origin.IsCompassActive);
            SelectedQuest = new ReactiveProperty<string>(origin.SelectedQuest);
            
            TrackedMissionID.Skip(1).Subscribe(v => origin.TrackedMissionID = v);
            IsCompassActive.Skip(1).Subscribe(v => origin.IsCompassActive = v);
            SelectedQuest.Skip(1).Subscribe(v => origin.SelectedQuest = v);
            
            InitQuests(origin);
            
            InitItems(origin);

            IsItemInHandsNull = new ReactiveProperty<bool>(origin.IsItemInHandsNull);
            IsItemInHandsNull.Skip(1).Subscribe(v => origin.IsItemInHandsNull = v);
            
            InHandsItemTypeId = new ReactiveProperty<string>(origin.InHandsItemTypeId);
            InHandsItemTypeId.Skip(1).Subscribe(v => origin.InHandsItemTypeId = v);

            IsPlayerInCar = new ReactiveProperty<bool>(origin.IsPlayerInCar);
            IsPlayerInCar.Skip(1).Subscribe(v => origin.IsPlayerInCar = v);

            IsCarDoorOpen = new ReactiveProperty<bool>(origin.IsCarDoorOpen);
            IsCarDoorOpen.Skip(1).Subscribe(v => origin.IsCarDoorOpen = v);

            InitResources(origin);
            InitItemsDestinations(origin);
        }

        public void Change(GameState origin)
        {
            //обязательно копируем, если это class (ссылки могут остаться)
            
            TrackedMissionID.Value = origin.TrackedMissionID;
            IsCompassActive.Value = origin.IsCompassActive;
            SelectedQuest.Value = origin.SelectedQuest;
            
            ActiveQuests.Clear();
            origin.ActiveQuests.ForEach(questData => ActiveQuests.Add(new QuestDataProxy(questData)));
            
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
            InHandsMarkId = origin.InHandsMarkId;
            
            ItemsInsideCar.Clear();
            origin.ItemsInsideCar.ForEach(questData => ItemsInsideCar.Add(new ItemClearProxy(questData)));

            IsPlayerInCar.Value = origin.IsPlayerInCar;
            IsCarDoorOpen.Value = origin.IsCarDoorOpen;
            
            Resources.Clear();
            origin.Resources.ForEach(questData => Resources.Add(new ResourceProxy(questData)));

            ItemsDestinationsPutters.Clear();
            foreach (var originItemsDestination in origin.ItemsDestinations)
                ItemsDestinationsPutters.Add(originItemsDestination.Key, new ItemDestinationProxy(originItemsDestination.Value));
        }
        
        /*public void Change(GameProxy gameProxy)
        {
            TrackedMissionID.Value = gameProxy.TrackedMissionID.Value;
            IsCompassActive.Value = gameProxy.IsCompassActive.Value;
        }*/
        
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
    }
}