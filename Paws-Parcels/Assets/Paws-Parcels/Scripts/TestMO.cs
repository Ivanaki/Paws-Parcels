using System.Collections;
using Quests;
using Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Paws_Parcels.Scripts
{
    public class TestMO : MonoBehaviour, IMessageHandler
    {
        [Inject] private MessageSystem _messageSystem;
        
        
        
        
        private IEnumerator Start()
        {
            _messageSystem.AddListener(this, "take", "box");
            _messageSystem.AddListener(this, "take", null);
            _messageSystem.AddListener(this, null, null);
            yield return null;
            _messageSystem.SendCompositeMessage(this, "take");
            yield return null;
            
            GetComponent<ContentSizeFitter>().SetLayoutVertical();
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

        public void OnMessage(MessageArgs messageArgs)
        {
            print("Get message: " + messageArgs.message + ":" + messageArgs.parameter);
        }
    }
}