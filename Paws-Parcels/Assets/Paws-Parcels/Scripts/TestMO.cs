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
        }

        [ReadOnly] public GameState gameState;
        
        private void Start()
        {
            gameState = _gameStateProvider.GameProxy.Origin;
            
            _gameStateProvider.GameProxy.SucceedQuests.ObserveAdd().Subscribe(e =>
            {
                print(e.Value);
            });
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