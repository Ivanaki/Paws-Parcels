using System.Linq;
using Messages;
using Paws_Parcels.NPCDialogs;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Services;
using UnityEngine;
using UnityEngine.UI;

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
        public ReadOnlyReactiveProperty<GameObject> Prefab { get; private set; }
        public string UiShowTypeId => "hint";

        public Vector3 GetGlobalPositionShowablePoint() => transform.position;
        public bool IsDestroyed() => this == null;

        public void SetReactive(ReactiveProperty<GameObject> im)
        {
            Prefab = im;
        }
    }

    public class HintsService
    {
        private readonly GameProxy _gameProxy;
        private PlayerHint _playerHint;
        private CarHint _carHint;

        private readonly ReactiveProperty<GameObject> _prefabImage = new(null);
        private readonly ReactiveProperty<Transform> _currentHintPosition = new(null);

        private readonly ReactiveProperty<bool> _isDialogActive = new(false);
        private readonly ReactiveProperty<bool> _isCutsceneActive = new(false);
        private readonly ReactiveProperty<bool> _isUIOpen = new(false);

        private GameObject _pendingPrefab;

        public HintsService(GameProxy gameProxy, MessageSystem messageSystem, EditorDataService editor,
            CanvasUiVisualizer canvasUiVisualizer, DialogueService dialogueService)
        {
            _gameProxy = gameProxy;
            gameProxy.IsPlayerInCar.Subscribe(_ => UpdateHint());

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
                if (image == null) canvasUiVisualizer.HideOnCanvas(t);
                else canvasUiVisualizer.ShowOnCanvas(t);
            });

            // --- Блокировка хинтов ---

            dialogueService.IsDialogRunning.Subscribe(val => _isDialogActive.Value = val);

            messageSystem.AddListenerWithManyMessages("cutscene_started").Subscribe(_ => _isCutsceneActive.Value = true);
            messageSystem.AddListenerWithManyMessages("cutscene_ended").Subscribe(_ => _isCutsceneActive.Value = false);

            messageSystem.AddListenerWithManyMessages(
                "open_screen_dialog/open_screen_map/open_screen_pause/open_screen_quests/open_popup_load_menu/open_popup_settings"
            ).Subscribe(_ => _isUIOpen.Value = true);
            messageSystem.AddListenerWithManyMessages("close_screen").Subscribe(_ => _isUIOpen.Value = false);

            var isBlocked = Observable.CombineLatest(
                _isDialogActive, _isCutsceneActive, _isUIOpen,
                (dialog, cutscene, ui) => dialog || cutscene || ui
            ).ToReadOnlyReactiveProperty();

            isBlocked.Subscribe(blocked =>
            {
                if (blocked)
                {
                    _prefabImage.Value = null;
                }
                else
                {
                    _prefabImage.Value = _pendingPrefab;
                }
            });

            // --- Показ/скрытие хинтов ---

            messageSystem.AddListenerWithManyMessages("show_hint").Subscribe(mes =>
            {
                var prefab = editor.EditorSettings.Hints.FirstOrDefault(x => x.Name == mes.parameter)?.Prefab;
                _pendingPrefab = prefab;
                if (!isBlocked.CurrentValue)
                    _prefabImage.Value = prefab;
            });

            messageSystem.AddListenerWithManyMessages("hide_hint").Subscribe(_ =>
            {
                _pendingPrefab = null;
                _prefabImage.Value = null;
            });
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
    }
}