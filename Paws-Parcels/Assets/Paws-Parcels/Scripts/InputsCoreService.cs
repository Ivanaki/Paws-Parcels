using System;
using System.Collections.Generic;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Services;
using Zenject;

namespace Paws_Parcels.Scripts
{
    public class InputsCoreService : IInitializable, IDisposable
    {
        private readonly GameInputService _gameInputService;
        private ReadOnlyReactiveProperty<bool> _isPlayerInCar { get; }
        private InputSystem_Actions _inputSystem { get; }

        private CompositeDisposable _disposable = new();
        private bool _stopAllUiActive = false;
        private bool _notDisableMovementUiActive = false;

        public InputsCoreService(GameStateProvider gameStateProvider, GameInputService gameInputService)
        {
            _gameInputService = gameInputService;
            _isPlayerInCar = gameStateProvider.GameProxy.IsPlayerInCar;
            _inputSystem = gameInputService.InputSystem;
        }

        public void Initialize()
        {
            _disposable.Add(_isPlayerInCar.Subscribe(_ => UpdateInputs()));
            _disposable.Add(_gameInputService.CutsceneInput.Subscribe(_ => UpdateInputs()));
            _disposable.Add(_gameInputService.DialogueInput.Subscribe(_ => UpdateInputs()));
            _disposable.Add(_gameInputService.AllGameplayInput.Subscribe(_ => UpdateInputs()));
            UpdateInputs();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }

        public void StopAllUiOpened()
        {
            _stopAllUiActive = true;
            UpdateInputs();
        }

        public void StopAllUiClosed()
        {
            _stopAllUiActive = false;
            UpdateInputs();
        }

        public void NotDisableMovementUiOpened()
        {
            _notDisableMovementUiActive = true;
            UpdateInputs();
        }

        public void NotDisableMovementUiClosed()
        {
            _notDisableMovementUiActive = false;
            UpdateInputs();
        }

        private void UpdateInputs()
        {
            if (_gameInputService.CutsceneInput.CurrentValue)
                _inputSystem.Cutscene.Enable();
            else
                _inputSystem.Cutscene.Disable();
            
            /*if (_gameInputService.DialogueInput.CurrentValue)
                _inputSystem.Cutscene.Enable();
            else
                _inputSystem.Cutscene.Disable();*/
            
            if (_gameInputService.AllGameplayInput.CurrentValue == false)
            {
                _inputSystem.UI.Disable();
                _inputSystem.Gameplay.Disable();
                _inputSystem.CarNotMovement.Disable();
                _inputSystem.Car.Disable();
                _inputSystem.PlayerNotMovement.Disable();
                _inputSystem.Player.Disable();
            }
            else
            {
                _inputSystem.Cutscene.Disable();
                
                if (_stopAllUiActive)
                {
                    _inputSystem.UI.Enable();
                    _inputSystem.Gameplay.Disable();

                    _inputSystem.CarNotMovement.Disable();
                    _inputSystem.Car.Disable();
                    _inputSystem.PlayerNotMovement.Disable();
                    _inputSystem.Player.Disable();
                }
                else
                {
                    if (_notDisableMovementUiActive)
                    {
                        _inputSystem.UI.Enable();
                        _inputSystem.Gameplay.Disable();

                        if (_isPlayerInCar.CurrentValue)
                        {
                            _inputSystem.CarNotMovement.Disable();
                            _inputSystem.Car.Enable();

                            _inputSystem.PlayerNotMovement.Disable();
                            _inputSystem.Player.Disable();
                        }
                        else
                        {
                            _inputSystem.PlayerNotMovement.Disable();
                            _inputSystem.Player.Enable();

                            _inputSystem.CarNotMovement.Disable();
                            _inputSystem.Car.Disable();
                        }
                    }
                    else
                    {
                        _inputSystem.UI.Disable();
                        _inputSystem.Gameplay.Enable();

                        if (_isPlayerInCar.CurrentValue)
                        {
                            _inputSystem.CarNotMovement.Enable();
                            _inputSystem.Car.Enable();

                            _inputSystem.PlayerNotMovement.Disable();
                            _inputSystem.Player.Disable();
                        }
                        else
                        {
                            _inputSystem.PlayerNotMovement.Enable();
                            _inputSystem.Player.Enable();

                            _inputSystem.CarNotMovement.Disable();
                            _inputSystem.Car.Disable();
                        }
                    }
                }
            }
        }
    }
}