using System;
using System.Collections;
using MotorHome.Scripts.Saves;
using R3;
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(ThirdPersonController))]
    public class PlayerApplySaves : MonoBehaviour
    {
        private ThirdPersonController _thirdPersonController;
        private GameStateProvider _gameStateProvider;
        private CompositeDisposable _disposable;
        private CharacterController _characterController;
        private ApplyInFixUpdate _applyInFixUpdate;
        
        private IDisposable _settingsStateProxyUpdated;
        
        [Inject]
        public void Construct(GameStateProvider gameStateProvider, ApplyInFixUpdate applyInFixUpdate)
        {
            _gameStateProvider = gameStateProvider;
            _applyInFixUpdate = applyInFixUpdate;
        }

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _thirdPersonController = GetComponent<ThirdPersonController>();
        }

        private void OnEnable()
        {
            _disposable = new CompositeDisposable();
            IDisposable dis;

            dis = _gameStateProvider.SettingsStateProxy.Subscribe(settingsStateProxy  =>
            {
                _settingsStateProxyUpdated?.Dispose();
                
                if(settingsStateProxy == null) return;
                
                _settingsStateProxyUpdated = settingsStateProxy.PlayerAim.Subscribe(value =>
                {
                    _thirdPersonController.SetSensivity(value);
                });
            }); _disposable.Add(dis);
            
            dis = _gameStateProvider.Updated.Subscribe(_ =>
            {
                _applyInFixUpdate.Apply().Take(1).Subscribe(_ =>
                {
                    ApplyState();
                });
            }); _disposable.Add(dis);

            dis = _gameStateProvider.Saved.Subscribe(_ =>
            {
                _gameStateProvider.CoreState.PlayerPosition = new Vector3DTO(_characterController.transform.localPosition);
                _gameStateProvider.CoreState.PlayerRotation = new QuaternionDTO(_characterController.transform.rotation);
                _gameStateProvider.CoreState.PlayerCameraRotationYaw = _thirdPersonController.GetRotationYawPitch().Item1;
                _gameStateProvider.CoreState.PlayerCameraRotationPitch = _thirdPersonController.GetRotationYawPitch().Item2;
            });_disposable.Add(dis);
        }
        
        private void ApplyState()
        {
            _characterController.transform.localPosition = _gameStateProvider.CoreState.PlayerPosition.ToUnity();
            _characterController.transform.rotation = _gameStateProvider.CoreState.PlayerRotation.ToUnity();
            _thirdPersonController.SetRotation(_gameStateProvider.CoreState.PlayerCameraRotationYaw,
                _gameStateProvider.CoreState.PlayerCameraRotationPitch);
        }
        
        private void OnDisable()
        {
            _disposable.Dispose();
        }
    }
}