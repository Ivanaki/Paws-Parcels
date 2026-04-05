using System.Linq;
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
        public Sprite Image;

        public HintData(string name, Sprite image)
        {
            Name = name;
            Image = image;
        }
    }

    public class Hint : MonoBehaviour, IShowableInUI
    {
        public ReadOnlyReactiveProperty<Sprite> Image { get; private set; }
        public string UiShowTypeId => "hint";
        public Vector3 GetGlobalPositionShowablePoint() => transform.position;
        public bool IsDestroyed() => this == null;

        public void SetReactive(ReactiveProperty<Sprite> im)
        {
            Image = im;
        }
    }
    
    public class HintsService
    {
        private readonly GameProxy _gameProxy;
        private PlayerHint _playerHint;
        private CarHint _carHint;

        private readonly ReactiveProperty<Sprite> _hintImage = new(null);
        
        private readonly ReactiveProperty<Transform> _currentHintPosition = new(null);

        public HintsService(GameProxy gameProxy, MessageSystem messageSystem, EditorDataService editor,
            CanvasUiVisualizer canvasUiVisualizer)
        {
            _gameProxy = gameProxy;
            gameProxy.IsPlayerInCar.Subscribe(_ => UpdateHint());

            var t = new GameObject("Hint").AddComponent<Hint>();
            t.SetReactive(_hintImage);
            
            _currentHintPosition.Subscribe(hintTransform =>
            {
                t.transform.SetParent(hintTransform);
                t.transform.localPosition = Vector3.zero;
                t.transform.localRotation = Quaternion.identity;
            });

            _hintImage.Subscribe(image =>
            {
                if (image == null) canvasUiVisualizer.HideOnCanvas(t);
                else canvasUiVisualizer.ShowOnCanvas(t);
            });

            messageSystem.AddListenerWithManyMessages("show_hint").Subscribe(mes =>
            {
                var image = editor.EditorSettings.Hints.FirstOrDefault(x => x.Name == mes.parameter)?.Image;
                _hintImage.Value = image;
            });

            messageSystem.AddListenerWithManyMessages("hide_hint").Subscribe(_ =>
            {
                _hintImage.Value = null;
            });
        }

        private void UpdateHint()
        {
            _currentHintPosition.Value = _gameProxy.IsPlayerInCar.Value ? _carHint?.transform : _playerHint?.transform;
        }
        
        public void SetPlayerHint(PlayerHint playerHint)
        {
            _playerHint = playerHint;
            UpdateHint();
            if (_currentHintPosition.Value == null)
                _currentHintPosition.Value = _playerHint.transform;
        }

        public void SetCarHint(CarHint carHint)
        {
            _carHint = carHint;
            UpdateHint();
            if (_currentHintPosition.Value == null)
                _currentHintPosition.Value = _carHint.transform;
        }
    }
}