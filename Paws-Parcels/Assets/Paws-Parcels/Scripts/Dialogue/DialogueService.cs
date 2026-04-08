using System;
using Extentions;
using Messages;
using NPCDialogs;
using ObservableCollections;
using Paws_Parcels.Scripts.Saves.States.GameResources;
using R3;
using Services;
using UnityEngine;
using Yarn.Unity;

namespace Paws_Parcels.NPCDialogs
{
    public class DialogueService
    {
        private readonly ShowableInterfaceService _showableInterfaceService;
        private readonly PlayerService _playerService;
        private readonly GameInputService _gameInputService;
        private readonly MessageSystem _messageSystem;

        private readonly IReadOnlyObservableDictionary<string, Action<DialogueRunner>> _commandsRegistrations;
        private readonly IReadOnlyObservableDictionary<string, Action<DialogueRunner>> _functionsRegistrations;

        public ReadOnlyReactiveProperty<bool> IsDialogRunning => _isDialogRunning;
        public string CurrentNpcDialog => _currentNpcDialog;

        private readonly ReactiveProperty<bool> _isDialogRunning = new(false);
        private string _currentNpcDialog;
        private DialogueRunner _currentRunner = null;

        public DialogueService(ShowableInterfaceService showableInterfaceService, PlayerService playerService,
            GameInputService gameInputService, DialogueRegistrations dialogueRegistrations,
            MessageSystem messageSystem)
        {
            _showableInterfaceService = showableInterfaceService;
            _playerService = playerService;
            _gameInputService = gameInputService;
            _messageSystem = messageSystem;

            _commandsRegistrations = dialogueRegistrations.CommandsRegistrations;
            _functionsRegistrations = dialogueRegistrations.FunctionsRegistrations;

            _commandsRegistrations.ObserveAdd().Subscribe(e =>
            {
                if (_currentRunner != null)
                {
                    var action = e.Value.Value;
                    action(_currentRunner);
                }
            });
            
            _commandsRegistrations.ObserveRemove().Subscribe(e =>
            {
                if (_currentRunner != null)
                {
                    _currentRunner.RemoveCommandHandler(e.Value.Key);
                }
            });
            
            _functionsRegistrations.ObserveAdd().Subscribe(e =>
            {
                if (_currentRunner != null)
                {
                    var action = e.Value.Value;
                    action(_currentRunner);
                }
            });
            
            _functionsRegistrations.ObserveRemove().Subscribe(e =>
            {
                if (_currentRunner != null)
                {
                    _currentRunner.RemoveFunction(e.Value.Key);
                }
            });
        }
        
        public async YarnTask<Observable<bool>> TryStartDialogue(DialogueReference dialogueReference, string npcNameCode,
            Transform npc, float duration)
        {
            if (_isDialogRunning.Value)
            {
                Debug.LogWarning("Dialogue service is already running");
                return Observable.Return(false);
            }

            if (dialogueReference == null)
            {
                Debug.LogWarning("Dialogue reference is null");
                return Observable.Return(false);
            }

            if (string.IsNullOrEmpty(dialogueReference.nodeName))
            {
                Debug.LogWarning("Dialogue reference node is null");
                return Observable.Return(false);
            }
            
            _showableInterfaceService.CloseAllInterface();
            _showableInterfaceService.OpenDialog();

            if (_currentRunner == null)
            {
                _showableInterfaceService.CloseAllInterface();
                Debug.LogWarning("DialogueRunner is null");
                return Observable.Return(false);
            }
            
            //_gameInputService.SetDialogueInput(true);
            //_gameInputService.SetAllGameplayInput(false);
            
            //_playerService.StopMouseAndInteract(this);
            //_playerService.StopMovement(this);
            _playerService.StartFixedTimeRotate(npc, duration);
            
            _showableInterfaceService.HideHud();
            _showableInterfaceService.BanOpen();
            
            _currentNpcDialog = npcNameCode;
            _isDialogRunning.Value = true;
            
            await YarnTask.Yield();
            await _currentRunner.StartDialogue(dialogueReference.nodeName);

            var endOfDialog = new Subject<Unit>();
            _currentRunner.onDialogueComplete.AsObservable().Take(1).Subscribe(x => 
                endOfDialog.OnNextAndDispose(Unit.Default));
            
            _messageSystem.SendCompositeMessage(this, "dialogue_started:" + dialogueReference.nodeName);
            
            endOfDialog.Subscribe(_ =>
            {
                //_gameInputService.SetDialogueInput(false);
                //_gameInputService.SetAllGameplayInput(true);
            
                //_playerService.StartMouseAndInteract(this);
                //_playerService.StartMovement(this);
                _playerService.StopFixedTimeRotate();
            
                _showableInterfaceService.ShowHud();
                _showableInterfaceService.UnbanOpen();
                
                _showableInterfaceService.CloseAllInterface();
            
                _currentNpcDialog = "";
                _isDialogRunning.Value = false;

                var message = "dialogue_ended:" + dialogueReference.nodeName;
                _messageSystem.SendCompositeMessage(this, message);
            });
            
            return endOfDialog.Select(x => true);
        }
        

        public async YarnTask<Observable<bool>> TryStartMonologue(DialogueReference dialogueReference)
        {
            if (_isDialogRunning.Value)
            {
                Debug.LogWarning("Dialogue service is already running");
                return Observable.Return(false);
            }

            if (dialogueReference == null)
            {
                Debug.LogWarning("Dialogue reference is null");
                return Observable.Return(false);
            }

            if (string.IsNullOrEmpty(dialogueReference.nodeName))
            {
                Debug.LogWarning("Dialogue reference node is null");
                return Observable.Return(false);
            }
            
            _showableInterfaceService.CloseAllInterface();
            _showableInterfaceService.OpenDialog();

            if (_currentRunner == null)
            {
                _showableInterfaceService.CloseAllInterface();
                Debug.LogWarning("DialogueRunner is null");
                return Observable.Return(false);
            }
            
            //_gameInputService.SetDialogueInput(true);
            //_gameInputService.SetAllGameplayInput(false);
            
            //_playerService.StopMouseAndInteract(this);
            //_playerService.StopMovement(this);
            //_playerService.StartFixedTimeRotate(npc, duration);
            
            _showableInterfaceService.HideHud();
            _showableInterfaceService.BanOpen();
            
            _currentNpcDialog = "";
            _isDialogRunning.Value = true;
            
            await YarnTask.Yield();
            await _currentRunner.StartDialogue(dialogueReference.nodeName);

            var endOfDialog = new Subject<Unit>();
            _currentRunner.onDialogueComplete.AsObservable().Take(1).Subscribe(x => 
                endOfDialog.OnNextAndDispose(Unit.Default));
            
            _messageSystem.SendCompositeMessage(this, "monologue_started:" + dialogueReference.nodeName);
            
            endOfDialog.Subscribe(_ =>
            {
                //_gameInputService.SetDialogueInput(false);
                //_gameInputService.SetAllGameplayInput(true);
            
                //_playerService.StartMouseAndInteract(this);
                //_playerService.StartMovement(this);
                //_playerService.StopFixedTimeRotate();
            
                _showableInterfaceService.ShowHud();
                _showableInterfaceService.UnbanOpen();
                
                _showableInterfaceService.CloseAllInterface();
            
                _currentNpcDialog = "";
                _isDialogRunning.Value = false;
                
                _messageSystem.SendCompositeMessage(this, "monologue_ended:" + dialogueReference.nodeName);
            });
            
            return endOfDialog.Select(x => true);
        }
        

        public void SetRunner(DialogueRunner runner)
        {
            if(runner == _currentRunner) return;
            
            _currentRunner = runner;
            
            if(runner == null) return;

            RegisterAll(_commandsRegistrations);
            RegisterAll(_functionsRegistrations);
        }

        private void RegisterAll(IReadOnlyObservableDictionary<string, Action<DialogueRunner>> registrations)
        {
            foreach (var pair in registrations)
            {
                pair.Value.Invoke(_currentRunner);
            }
        }
    }
}