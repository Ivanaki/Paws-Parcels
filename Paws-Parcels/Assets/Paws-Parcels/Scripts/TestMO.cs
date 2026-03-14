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
        [Inject] private CutsceneService _cutsceneService;
        [Inject] private LocalizationService _localizationService;
        
        
        public void Send(string message)
        {
            _messageSystem.SendCompositeMessage(this, message);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                _cutsceneService.StartCutScene("test");
            }
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
            var t = _localizationService.GetCurrentLanguageStringOrDefault("test", Tables.Dialogs);
            
            gameState = _gameStateProvider.GameProxy.Origin;
            settingsState = _settingsStateProvider.SettingsProxy.SettingsState;
            
            _gameStateProvider.GameProxy.SucceedQuests.ObserveAdd().Subscribe(e =>
            {
                print(e.Value);
            });
            
            _questsService.ActiveNewQuest("bulka_sova");
            _questsService.ActiveNewQuest("bulka_sova_t");
            //_questsService.ActiveNewQuest("Test");
            _gameStateProvider.GameProxy.SelectedQuest.Value = "bulka_sova";
            //print(LocalizationSettings.SelectedLocale.CustomFormatterCode);
            //print(LocalizationSettings.SelectedLocale.LocaleName);
            //print(LocalizationSettings.SelectedLocale.name);
            //print(LocalizationSettings.SelectedLocale.Identifier.Code);
            //print(LocalizationSettings.SelectedLocale.Identifier.CultureInfo.Name);
            //print(LocalizationSettings.SelectedLocale.Identifier.CultureInfo.Name);

            //_mouseEnterUtil.IsMouseEnter.Subscribe(v => print(v));
            //_gameStateProvider.GameProxy.TrackedMissionID.Value = 5;
            //_gameStateProvider.SaveGameState(true);
            //var callback = _gameStateProvider.DeleteGameState(Guid.Parse(_gameStateProvider.GetAllSaveMeta()[0].SaveId));
            //var callback = _gameStateProvider.LoadGameState(Guid.NewGuid());
            //print(callback.MessageToUser);
            //print(callback.Exception.Message);
            //_gameStateProvider.LoadNewGame();
            //print(_gameStateProvider.GameProxy.TrackedMissionID.Value);
        }
        
    }
}