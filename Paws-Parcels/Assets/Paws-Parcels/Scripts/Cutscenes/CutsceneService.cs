using System;
using System.Collections.Generic;
using Paws_Parcels.Scripts.Player;
using R3;
using UnityEngine;

namespace Services
{
    public class CutsceneService
    {
        private readonly PlayerShowableService _playerShowableService;
        private readonly ShowableInterfaceService _showableInterfaceService;
        private readonly GameInputService _gameInputService;
        private readonly MessageSystem _messageSystem;

        private readonly Dictionary<string, Cutscene> _cutscenesDict = new();
        private readonly Dictionary<string, IDisposable> _cutscenesSubscriptionsDict = new();
        private bool _cutscenePlaying = false;

        public CutsceneService(PlayerShowableService playerShowableService, ShowableInterfaceService showableInterfaceService,
            GameInputService gameInputService, MessageSystem messageSystem)
        {
            _playerShowableService = playerShowableService;
            _showableInterfaceService = showableInterfaceService;
            _gameInputService = gameInputService;
            _messageSystem = messageSystem;
        }
        
        public Observable<bool> StartCutScene(string cutsceneId)
        {
            if (_cutscenesDict.ContainsKey(cutsceneId) == false)
            {
                Debug.LogError($"Cutscene not found: {cutsceneId}");
                return Observable.Return(false);
            }

            if (_cutscenePlaying)
            {
                Debug.LogError($"Some cutscene already playing");
                return Observable.Return(false);
            }
            
            var cutscene = _cutscenesDict[cutsceneId];
            
            //спрятать: игрока
            if (cutscene.HidePlayer)
                _playerShowableService.Hide();
            
            //остановить ввод инпута всего, кроме кадсценовского
            _gameInputService.SetAllGameplayInput(false);
            _gameInputService.SetCutsceneInput(true);
            
            //закрыть и отключить возможность открывать интерфейс
            _showableInterfaceService.CloseAllInterface();
            _showableInterfaceService.BanOpen();
            //спрятать интерфейс
            _showableInterfaceService.HideHud();
            
            _cutscenePlaying = true;
            
            _messageSystem.SendCompositeMessage(this, "cutscene_started:" + cutsceneId);

            var sub = cutscene.Play();
            sub.Subscribe(_ =>
            {
                _cutscenePlaying = false;
                
                if (cutscene.HidePlayer)
                    _playerShowableService.Show();
            
                _gameInputService.SetAllGameplayInput(true);
                _gameInputService.SetCutsceneInput(false);
                _showableInterfaceService.UnbanOpen();
                _showableInterfaceService.ShowHud();
                _messageSystem.SendCompositeMessage(this, "cutscene_ended:" + cutsceneId);

            });
            
            return sub.Select(_ => true);
            //пока не буду добавлять: спрятать машину, items на машине и в ней, отдельные items
            //return Observable.Return(false);
        }
        
        public void RegisterCutscene(Cutscene cutscene)
        {
            if (_cutscenesDict.ContainsKey(cutscene.CutsceneId))
            {
                Debug.LogError($"Duplicate cutscene id: {cutscene.CutsceneId}. Cutscene dont register.");
                return;
            }
            
            _cutscenesSubscriptionsDict[cutscene.CutsceneId] = 
                _messageSystem.AddListenerWithManyMessages("start_cutscene-" + cutscene.CutsceneId);
            _cutscenesDict.Add(cutscene.CutsceneId, cutscene);
        }

        public void UnregisterCutscene(Cutscene cutscene)
        {
            if (_cutscenesDict.ContainsKey(cutscene.CutsceneId) == false)
            {
                Debug.LogError($"Cutscene not found: {cutscene.CutsceneId}");
                return;
            }
            
            _cutscenesSubscriptionsDict[cutscene.CutsceneId].Dispose();
            _cutscenesSubscriptionsDict.Remove(cutscene.CutsceneId);
            _cutscenesDict.Remove(cutscene.CutsceneId);
        }
    }
}