using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using ObservableCollections;
using Paws_Parcels.Scripts.Quests;
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

        //public 

        public void Send(string message)
        {
            _messageSystem.SendCompositeMessage(this, message);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                _messageSystem.SendCompositeMessage(this, "f1");
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                _messageSystem.SendCompositeMessage(this, "f2");
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                _messageSystem.SendCompositeMessage(this, "f3");
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                _messageSystem.SendCompositeMessage(this, "t1");
                _messageSystem.SendCompositeMessage(this, "A");
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                _messageSystem.SendCompositeMessage(this, "t2");
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                _messageSystem.SendCompositeMessage(this, "t3");
            }
            
            if (Input.GetKeyDown(KeyCode.O))
            {
                _messageSystem.SendCompositeMessage(this, "On");
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                _messageSystem.SendCompositeMessage(this, "Off");
            }
            
            if (Input.GetKeyDown(KeyCode.S))
            {
                _gameStateProvider.GameProxy.SelectedQuest.Value = "Gasoline";
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                _gameStateProvider.GameProxy.SelectedQuest.Value = "None";
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                _messageSystem.SendCompositeMessage(this, "1");
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                _messageSystem.SendCompositeMessage(this, "2");
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                _messageSystem.SendCompositeMessage(this, "3");
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                _messageSystem.SendCompositeMessage(this, "4");
            }
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                _messageSystem.SendCompositeMessage(this, "5");
            }
            
        }

        [ReadOnly] public GameState gameState;
        
        private void Start()
        {
            gameState = _gameStateProvider.GameProxy.Origin;
            
            _gameStateProvider.GameProxy.SucceedQuests.ObserveAdd().Subscribe(e =>
            {
                print(e.Value);
            });
            
            _questsService.ActiveNewQuest("Gasoline");
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