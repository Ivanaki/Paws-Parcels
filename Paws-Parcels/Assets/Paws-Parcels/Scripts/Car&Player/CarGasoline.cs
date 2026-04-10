using Paws_Parcels.Car.Logic;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Car
{
    public class CarGasoline : MonoBehaviour
    {
        public bool HasGas => _hasGas;

        private GameProxy _gameProxy;
        private CompositeDisposable _disposable;
        private CarInput _carInput;
        private bool _hasGas = true;

        [Inject]
        private void Construct(GameProxy gameProxy)
        {
            _gameProxy = gameProxy;
            _hasGas = gameProxy.HasGas.Value;
        }

        private void Awake()
        {
            _carInput = GetComponent<CarInput>();
        }

        private void OnEnable()
        {
            _disposable = new CompositeDisposable();
            if (_gameProxy == null)
            {
                return;
            }

            _disposable.Add(_gameProxy.HasGas.Subscribe(SetGasState));
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }

        private void SetGasState(bool hasGas)
        {
            _hasGas = hasGas;

            if (hasGas == false)
            {
                ResetInput();
            }
        }

        private void ResetInput()
        {
            if (_carInput == null)
            {
                return;
            }

            _carInput.Steer = 0f;
            _carInput.Throttle = 0f;
            _carInput.Handbrake = false;
        }
    }
}
