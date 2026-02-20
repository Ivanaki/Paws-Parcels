using System;
using Paws_Parcels.Scripts.Saves.States;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;

using UnityEngine;
using Zenject;

namespace Paws_Parcels.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(ThirdPersonController))]
    public class PlayerApplySaves : MonoBehaviour
    {
        private GameStateProvider _gameStateProvider;
        private ApplyInFixUpdate _applyInFixUpdate;
        private ReadOnlyReactiveProperty<float> _sensitivity;
        
        private ThirdPersonController _thirdPersonController;
        private CharacterController _characterController;
        
        private CompositeDisposable _disposable;
        
        [Inject]
        public void Construct(GameStateProvider gameStateProvider, ApplyInFixUpdate applyInFixUpdate,
            SettingsStateProvider settingsStateProvider)
        {
            _gameStateProvider = gameStateProvider;
            _applyInFixUpdate = applyInFixUpdate;
            _sensitivity = settingsStateProvider.SettingsProxy.Sensitivity;
        }

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _thirdPersonController = GetComponent<ThirdPersonController>();
        }

        private void OnEnable()
        {
            _disposable = new CompositeDisposable();

            _disposable.Add(_sensitivity.Subscribe(value  =>
                _thirdPersonController.SetSensivity(value)));
            
            _disposable.Add(_gameStateProvider.ApplyDataAfterLoad.Subscribe(_ =>
            {
                _applyInFixUpdate.ObserveApply().Take(1).Subscribe(_ =>
                {
                    var playerData = _gameStateProvider.GameProxy.PlayerData;
                    _characterController.transform.position = playerData.Position;
                    _characterController.transform.rotation = playerData.Rotation;
                    _thirdPersonController.SetRotation(playerData.Yaw, playerData.Pitch);
                });
            }));

            _disposable.Add(_gameStateProvider.CollectDataBeforeSave.Subscribe(_ =>
            {
                _gameStateProvider.GameProxy.PlayerData = new PlayerSaveData()
                {
                    Position = _characterController.transform.position,
                    Rotation = _characterController.transform.rotation,
                    Yaw = _thirdPersonController.GetTargetYaw(),
                    Pitch = _thirdPersonController.GetTargetPitch()
                };
            }));
        }
        
        private void OnDisable()
        {
            _disposable?.Dispose();
        }
    }
}