using System;
using R3;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class SoloMonolog : MonoBehaviour
    {
        [SerializeField] private Transform _playerParent;
        [SerializeField] private Transform _carParent;
        
        [SerializeField] private DialogShower _dialogShower;

        public DialogShower DialogShower => _dialogShower;
        public bool IsStarted { private set; get; } = false;
        
        private CarPlayerSwitcher _carPlayerSwitcher;
        private IDisposable _disposable = null;
        private IStopMove _stopMove;
        
        private Transform _currentTransform;
        
        [Inject]
        private void Construct(CarPlayerSwitcher carPlayerSwitcher, IStopMove stopMove)
        {
            _carPlayerSwitcher = carPlayerSwitcher;
            _stopMove = stopMove;
        }
        
        private void OnEnable()
        {
            _disposable = _carPlayerSwitcher.CurrentState.Subscribe(value =>
            {
                _currentTransform = value == CarPlayerSwitcher.CarOrPlayerState.Car
                    ? _carParent
                    : _playerParent;

                _dialogShower.transform.localPosition = Vector3.zero;
            });
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }

        public void Click()
        {
            _dialogShower.Click();
        }
        
        public Observable<Unit> StartMonolog(string[] phases)
        {
            //_stopMove.
            var observable = _dialogShower.Show(phases);
            observable.Take(1).Subscribe(_ => IsStarted = false);
            IsStarted = true;
            return observable;
        }

        private void Update()
        {
            _dialogShower.transform.position = _currentTransform.position;
        }
    }
}