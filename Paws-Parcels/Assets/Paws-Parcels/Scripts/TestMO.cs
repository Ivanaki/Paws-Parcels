using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using ObservableCollections;
using Paws_Parcels.Scripts.Quests;
using Paws_Parcels.Scripts.Saves.States;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Quests;
using R3;
using Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Paws_Parcels.Scripts
{
    public class TestMO : MonoBehaviour
    {
        [Inject] private MessageSystem _messageSystem;
        [Inject] private QuestsService _questsService;
        [Inject] private GameStateProvider _gameStateProvider;
        [Inject] private SettingsStateProvider _settingsStateProvider;
        
        
        public void Send(string message)
        {
            _messageSystem.SendCompositeMessage(this, message);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                _messageSystem.SendCompositeMessage(this, "On");
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                _messageSystem.SendCompositeMessage(this, "Off");
            }
            
            if (Input.GetKeyDown(KeyCode.Z))
            {
                _gameStateProvider.GameProxy.SelectedQuest.Value = "bulka_sova";
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                _gameStateProvider.GameProxy.SelectedQuest.Value = "bulka_sova_t";
            }
        }

        [ReadOnly] public GameState gameState;
        [ReadOnly] public SettingsState settingsState;
        
        private void Start()
        {
            gameState = _gameStateProvider.GameProxy.Origin;
            settingsState = _settingsStateProvider.SettingsProxy.SettingsState;
            
            _questsService.ActiveNewQuest("bulka_sova");
            _questsService.ActiveNewQuest("bulka_sova_t");
            
            _gameStateProvider.GameProxy.SelectedQuest.Value = "bulka_sova";
        }
        
    }
}