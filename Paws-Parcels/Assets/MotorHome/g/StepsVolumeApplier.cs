using System;
using MotorHome.Scripts.Saves;
using StarterAssets;
using UnityEngine;
using Zenject;
using R3;

namespace MotorHome.Scripts
{
    [RequireComponent(typeof(ThirdPersonController))]
    public class StepsVolumeApplier : MonoBehaviour
    {
        private ThirdPersonController _thirdPersonController;
        
        private GameStateProvider _gameStateProvider;

        private IDisposable _disposable;
        private IDisposable _settingsStateProxyUpdated;
        
        [Inject]
        private void Construct(GameStateProvider  gameStateProvider)
        {
            _gameStateProvider = gameStateProvider;
        }

        private void Awake()
        {
            _thirdPersonController = GetComponent<ThirdPersonController>();
        }

        private void OnEnable()
        {
            _disposable = _gameStateProvider.SettingsStateProxy.Subscribe(settingsStateProxy =>
            {
                _settingsStateProxyUpdated?.Dispose();
                
                if(settingsStateProxy == null) return;
                
                _settingsStateProxyUpdated = settingsStateProxy.MasterVolume.Subscribe(value =>
                {
                    _thirdPersonController.FootstepAudioVolume = value;
                });
            });
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }
    }
}