using System;
using ObservableCollections;
using Paws_Parcels.Scripts.Quests;
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
        }

        public void Change(GameState gameState)
        {
            TrackedMissionID.Value = gameState.TrackedMissionID;
            IsCompassActive.Value = gameState.IsCompassActive;
            SelectedQuest.Value = gameState.SelectedQuest;
            
            ActiveQuests.Clear();
            gameState.ActiveQuests.ForEach(questData => ActiveQuests.Add(new QuestDataProxy(questData)));
            
            SucceedQuests.Clear();
            gameState.SucceedQuests.ForEach(questData => SucceedQuests.Add(questData));
            
            CurrentActiveDestinations.Clear();
            gameState.CurrentActiveDestinations.ForEach(questData => CurrentActiveDestinations.Add(questData));
            
            PlayerData = gameState.PlayerData;
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
        
        private void InitQuests(GameState gameState)
        {
            gameState.ActiveQuests.ForEach(questData => ActiveQuests.Add(new QuestDataProxy(questData)));
            
            ActiveQuests.ObserveAdd().Subscribe(e =>
            {
                var addedResource = e.Value;
                gameState.ActiveQuests.Add(addedResource.Origin);
            });
            
            ActiveQuests.ObserveRemove().Subscribe(e =>
            {
                var removedResource = e.Value;
                gameState.ActiveQuests.Remove(removedResource.Origin);
            });
            
            ActiveQuests.ObserveClear().Subscribe(e =>
            {
                gameState.ActiveQuests.Clear();
            });
            
            gameState.SucceedQuests.ForEach(questData => SucceedQuests.Add(questData));
            
            SucceedQuests.ObserveAdd().Subscribe(e =>
            {
                var addedResource = e.Value;
                gameState.SucceedQuests.Add(addedResource);
            });
            
            SucceedQuests.ObserveRemove().Subscribe(e =>
            {
                var removedResource = e.Value;
                gameState.SucceedQuests.Remove(removedResource);
            });
            
            SucceedQuests.ObserveClear().Subscribe(e =>
            {
                gameState.SucceedQuests.Clear();
            });
            
            gameState.CurrentActiveDestinations.ForEach(questData => CurrentActiveDestinations.Add(questData));
            
            CurrentActiveDestinations.ObserveAdd().Subscribe(e =>
            {
                var addedResource = e.Value;
                gameState.CurrentActiveDestinations.Add(addedResource);
            });
            
            CurrentActiveDestinations.ObserveRemove().Subscribe(e =>
            {
                var removedResource = e.Value;
                gameState.CurrentActiveDestinations.Remove(removedResource);
            });
            
            CurrentActiveDestinations.ObserveClear().Subscribe(e =>
            {
                gameState.CurrentActiveDestinations.Clear();
            });
        }
    }
}