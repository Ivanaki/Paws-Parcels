using System;
using System.Collections.Generic;
using NaughtyAttributes;
using R3;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Paws_Parcels.Scripts
{
    /*[Serializable]
    public class Te
    {
        [SerializeField] private bool test;

        [SerializeField, AllowNesting, HideIf(nameof(test))] private List<string> list;
    }*/
    
    public class TestMO : MonoBehaviour
    {
        //[SerializeField] private MouseEnterUtil _mouseEnterUtil;

        //[SerializeField, AllowNesting] private Te list;
        
        
        private void Start()
        {
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