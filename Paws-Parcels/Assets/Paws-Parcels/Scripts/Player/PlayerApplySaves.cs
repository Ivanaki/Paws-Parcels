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
        [SerializeField] private GameObject _modelToHide;
        
        private GameStateProvider _gameStateProvider;
        private ApplyInFixUpdate _applyInFixUpdate;
        private ReadOnlyReactiveProperty<float> _sensitivity;
        
        private ThirdPersonController _thirdPersonController;
        private CharacterController _characterController;
        
        private CompositeDisposable _disposable;
        private Transform _spawnPositionOutCar;
        
        [Inject]
        public void Construct(GameStateProvider gameStateProvider, ApplyInFixUpdate applyInFixUpdate,
            SettingsStateProvider settingsStateProvider)
        {
            _gameStateProvider = gameStateProvider;
            _applyInFixUpdate = applyInFixUpdate;
            _sensitivity = settingsStateProvider.SettingsProxy.PlayerSensitivity;
        }

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _thirdPersonController = GetComponent<ThirdPersonController>();
            GetSpawnPosition();
        }

        private void OnEnable()
        {
            GetSpawnPosition();
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
            
            _disposable.Add(_gameStateProvider.GameProxy.IsPlayerInCar.Subscribe(value =>
            {
                _thirdPersonController.enabled = !value;
                _characterController.enabled = !value;
                _modelToHide.SetActive(!value);
                if (value == false)
                {
                    GetSpawnPosition();
                    if (_spawnPositionOutCar != null)
                    {
                        _characterController.transform.position = _spawnPositionOutCar.position;
                    }
                }
            }));
        }
        
        private void OnDisable()
        {
            _disposable?.Dispose();
        }

        private void GetSpawnPosition()
        {
            if (_spawnPositionOutCar == null)
            {
                _spawnPositionOutCar = GameObject.FindGameObjectWithTag("PlayerOutCarPosition").transform;
            }
        }
    }
}