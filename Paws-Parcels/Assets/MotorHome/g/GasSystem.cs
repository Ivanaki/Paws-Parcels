using System;
using MotorHome.Scripts.Saves;
using R3;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class GasSystem : IInitializable, IDisposable
    {
        private readonly GasUI _gasUI;
        private readonly CarPlayerSwitcher _carPlayerSwitcher;
        private readonly GameStateProvider _gameStateProvider;
        private float _gasValue;

        private IDisposable _disposable;
        
        public GasSystem(CoreUI coreUI, CarPlayerSwitcher carPlayerSwitcher, GameStateProvider gameStateProvider)
        {
            _gasUI = coreUI.GasUI;
            _carPlayerSwitcher = carPlayerSwitcher;
            _gameStateProvider = gameStateProvider;
        }

        public void SetGas(float value)
        {
            _gasValue = value;
            value = Mathf.Clamp01(value);
            _gasUI.UpdateUI(value);
        }

        public void Initialize()
        {
            _disposable = _carPlayerSwitcher.CurrentState.Subscribe(value =>
            {
                if (value == CarPlayerSwitcher.CarOrPlayerState.Car)
                {
                    _gasUI.Activate();
                }
                else
                {
                    _gasUI.Deactivate();
                }
            });

            var dis = _gameStateProvider.Saved.Subscribe(_ =>
            {
                _gameStateProvider.CoreState.GasValue = _gasValue;
            }); _disposable = Disposable.Combine(_disposable, dis);
            
            dis = _gameStateProvider.Updated.Subscribe(_ =>
            {
                SetGas(_gameStateProvider.CoreState.GasValue);
            }); _disposable = Disposable.Combine(_disposable, dis);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}