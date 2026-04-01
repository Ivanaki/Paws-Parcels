using System;
using System.Collections.Generic;
using System.Linq;
using ObservableCollections;
using Paws_Parcels.Scripts.Quests;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Quests;
using R3;
using UnityEngine;
using Zenject;

namespace Map
{
    public class BuildingsLogic
    {
        private readonly Dictionary<string, Building> _buildings = new();

        private readonly GameProxy _gameProxy;
        private readonly QuestsDatabase _questsDatabase;
        
        public void Register(Building building)
        {
            if (building == null)
            {
                Debug.LogError("[BuildingsLogic] Register failed: building is null.");
                return;
            }

            if (building.gameObject == null)
            {
                Debug.LogError("[BuildingsLogic] Register failed: building.gameObject is null.");
                return;
            }

            if (string.IsNullOrEmpty(building.MapShowTypeId))
            {
                Debug.LogError($"[BuildingsLogic] Register failed: MapShowTypeId is null or empty. GameObject: {building.gameObject.name}");
                return;
            }

            var result = _buildings.TryAdd(building.MapShowTypeId, building);

            if (result)
            {
                //Debug.Log($"[BuildingsLogic] Registered building. Id: {building.MapShowTypeId}, GameObject: {building.gameObject.name}");
            }
            else
            {
                var existingBuilding = _buildings[building.MapShowTypeId];
                var existingName = existingBuilding != null && existingBuilding.gameObject != null
                    ? existingBuilding.gameObject.name
                    : "null";

                Debug.LogError(
                    $"[BuildingsLogic] Register failed: duplicate MapShowTypeId found. " +
                    $"Id: {building.MapShowTypeId}, New: {building.gameObject.name}, Existing: {existingName}");
            }
            
            UpdateUi();
        }

        public void Unregister(Building building)
        {
            if (building == null)
            {
                Debug.LogError("[BuildingsLogic] Unregister failed: building is null.");
                return;
            }

            if (string.IsNullOrEmpty(building.MapShowTypeId))
            {
                Debug.LogError("[BuildingsLogic] Unregister failed: MapShowTypeId is null or empty.");
                return;
            }

            if (!_buildings.TryGetValue(building.MapShowTypeId, out var registeredBuilding))
            {
                Debug.LogWarning($"[BuildingsLogic] Unregister skipped: building with Id '{building.MapShowTypeId}' is not registered.");
                return;
            }

            if (registeredBuilding != building)
            {
                var passedName = building.gameObject != null ? building.gameObject.name : "null";
                var registeredName = registeredBuilding != null && registeredBuilding.gameObject != null
                    ? registeredBuilding.gameObject.name
                    : "null";

                Debug.LogError(
                    $"[BuildingsLogic] Unregister failed: registered building does not match passed instance. " +
                    $"Id: {building.MapShowTypeId}, Passed: {passedName}, Registered: {registeredName}");
                return;
            }

            _buildings.Remove(building.MapShowTypeId);
            //Debug.Log($"[BuildingsLogic] Unregistered building. Id: {building.MapShowTypeId}");
            
            UpdateUi();
        }
        
        private string _currentShowingBuildingId = null;
        
        private CompositeDisposable _disposables;

        public BuildingsLogic(GameProxy gameProxy, QuestsDatabase questsDatabase)
        {
            _gameProxy = gameProxy;
            _questsDatabase = questsDatabase;
            gameProxy.SelectedQuest.Subscribe(_ => UpdateUi());
            //gameProxy.ActiveQuests.ObserveChanged().Subscribe(_ => UpdateUi());
        }
        
        private void UpdateUi()
        {
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();
            
            if (string.IsNullOrEmpty(_currentShowingBuildingId) == false)
            {
                if(_buildings.TryGetValue(_currentShowingBuildingId, out var building))
                {
                    building.SetSelected(false);
                }
            }

            var selectedQuest = _gameProxy.SelectedQuest.CurrentValue; 
            
            if (string.IsNullOrEmpty(selectedQuest) == false)
            {
                var questDataProxy = _gameProxy.ActiveQuests.FirstOrDefault(x => x.Id == selectedQuest);
                
                if (questDataProxy != null)
                {
                    var selectedQuestMission = questDataProxy.CurrentMission.Value;
                    _disposables.Add(questDataProxy.CurrentMission.Skip(1).Take(1).Subscribe(_ => UpdateUi()));
                    
                    if (_questsDatabase.QuestsDict.TryGetValue(selectedQuest, out var quest))
                    {
                        if (quest.Missions[selectedQuestMission].HasBuildingDestination)
                        {
                            var newShowingBuildingId = quest.Missions[selectedQuestMission].BuildingDestinationId;
                            
                            if(_buildings.TryGetValue(newShowingBuildingId, out var building))
                            {
                                building.SetSelected(true);
                                _currentShowingBuildingId = newShowingBuildingId;
                            }
                        }
                    }
                }
            }
        }
    }
}