using System;
using System.Linq;
using DefaultNamespace;
using Messages;
using Paws_Parcels.NPCDialogs;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace Car_Player
{
    [System.Serializable]
    public class HintData
    {
        public string Name;
        public GameObject Prefab;

        public HintData(string name, GameObject prefab)
        {
            Name = name;
            Prefab = prefab;
        }
    }

    public class Hint : MonoBehaviour, IShowableInUI
    {
        public BehaviorSubject<GameObject> Prefab { get; private set; }
        public string UiShowTypeId => "hint";

        public Vector3 GetGlobalPositionShowablePoint() => transform.position;
        public bool IsDestroyed() => this == null;

        public void SetReactive(BehaviorSubject<GameObject> im)
        {
            Prefab = im;
        }
    }

    public class HintsService : IInitializable, IDisposable
    {
        private readonly GameProxy _gameProxy;
        private readonly MessageSystem _messageSystem;
        private readonly CanvasUiVisualizer _canvasUiVisualizer;
        private readonly DialogueService _dialogueService;
        private PlayerHint _playerHint;
        private CarHint _carHint;

        private readonly BehaviorSubject<GameObject> _prefabImage = new(null);
        private readonly ReactiveProperty<Transform> _currentHintPosition = new(null);

        private readonly ReactiveProperty<bool> _isDialogActive = new(false);
        private readonly ReactiveProperty<bool> _isCutsceneActive = new(false);
        private readonly ReactiveProperty<bool> _isUIOpen = new(false);

        private GameObject _pendingPrefab;

        private CompositeDisposable _disposables;

        public void Initialize()
        {
            _disposables = new CompositeDisposable();
            
            _gameProxy.IsPlayerInCar.Subscribe(_ => UpdateHint());

            var t = new GameObject("Hint").AddComponent<Hint>();
            t.SetReactive(_prefabImage);

            _currentHintPosition.Subscribe(hintTransform =>
            {
                if (t == null) return;
                t.transform.SetParent(hintTransform);
                t.transform.localPosition = Vector3.zero;
                t.transform.localRotation = Quaternion.identity;
            });

            _prefabImage.Subscribe(image =>
            {
                if (image == null) _canvasUiVisualizer.HideOnCanvas(t);
                else _canvasUiVisualizer.ShowOnCanvas(t);
            });

            // --- Блокировка хинтов ---

            _dialogueService.IsDialogRunning.Subscribe(val => _isDialogActive.Value = val);

            _messageSystem.AddListenerWithManyMessages("cutscene_started").Subscribe(_ => _isCutsceneActive.Value = true);
            _messageSystem.AddListenerWithManyMessages("cutscene_ended").Subscribe(_ => _isCutsceneActive.Value = false);

            _messageSystem.AddListenerWithManyMessages(
                "open_screen_dialog/open_screen_map/open_screen_pause/open_screen_loose/open_screen_end_game/open_screen_quests/open_popup_load_menu/open_popup_settings"
            ).Subscribe(_ => _isUIOpen.Value = true);
            _messageSystem.AddListenerWithManyMessages("close_screen").Subscribe(_ => _isUIOpen.Value = false);

            var isBlocked = Observable.CombineLatest(
                _isDialogActive, _isCutsceneActive, _isUIOpen,
                (dialog, cutscene, ui) => dialog || cutscene || ui
            ).ToReadOnlyReactiveProperty();

            isBlocked.Subscribe(blocked =>
            {
                if (blocked)
                {
                    _prefabImage.OnNext(null);
                }
                else
                {
                    _prefabImage.OnNext(_pendingPrefab);
                }
            });

            // --- Показ/скрытие хинтов ---

            _messageSystem.AddListenerWithManyMessages("show_hint").Subscribe(mes =>
            {
                //var prefab = editor.EditorSettings.Hints.FirstOrDefault(x => x.Name == mes.parameter)?.Prefab;
                var prefab = HintsData.instance?.Hints?.FirstOrDefault(x => x.Name == mes.parameter)?.Prefab;
                _pendingPrefab = prefab;
                //if (!isBlocked.CurrentValue)
                if (_prefabImage.Value == prefab)
                {
                    
                }

                _prefabImage.OnNext(prefab);
            });

            _messageSystem.AddListenerWithManyMessages("hide_hint").Subscribe(_ =>
            {
                _pendingPrefab = null;
                _prefabImage.OnNext(null);
            });
        }
        
        public HintsService(GameProxy gameProxy, MessageSystem messageSystem, EditorDataService editor,
            CanvasUiVisualizer canvasUiVisualizer, DialogueService dialogueService)
        {
            _gameProxy = gameProxy;
            _messageSystem = messageSystem;
            _canvasUiVisualizer = canvasUiVisualizer;
            _dialogueService = dialogueService;
        }

        private void UpdateHint()
        {
            _currentHintPosition.Value = _gameProxy.IsPlayerInCar.Value
                ? (_carHint != null ? _carHint.transform : null)
                : (_playerHint != null ? _playerHint.transform : null);
        }
        
        public void SetPlayerHint(PlayerHint playerHint)
        {
            _playerHint = playerHint;
            UpdateHint();
            if (_currentHintPosition.Value == null && _playerHint != null)
                _currentHintPosition.Value = _playerHint.transform;
        }

        public void SetCarHint(CarHint carHint)
        {
            _carHint = carHint;
            UpdateHint();
            if (_currentHintPosition.Value == null && _carHint != null)
                _currentHintPosition.Value = _carHint.transform;
        }

        public void Dispose()
        {
            _prefabImage?.Dispose();
            _currentHintPosition?.Dispose();
            _isDialogActive?.Dispose();
            _isCutsceneActive?.Dispose();
            _isUIOpen?.Dispose();
            _disposables?.Dispose();
        }

       
    }
}
