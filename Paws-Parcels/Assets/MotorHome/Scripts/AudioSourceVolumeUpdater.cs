using System;
using MotorHome.Scripts.Saves;
using R3;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceVolumeUpdater : MonoBehaviour
    {
        [Inject] private GameStateProvider _gameStateProvider;
        
        private CompositeDisposable _disposable;
        private IDisposable _settingsStateProxyUpdated;
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        
        private void OnEnable()
        {
            IDisposable dis;
            
            dis = _gameStateProvider.SettingsStateProxy.Subscribe(settingsStateProxy =>
            {
                _settingsStateProxyUpdated?.Dispose();
                
                if(settingsStateProxy == null) return;
                
                _settingsStateProxyUpdated = settingsStateProxy.MasterVolume.Subscribe(value =>
                {
                    _audioSource.volume = value;
                });
            }); _disposable?.Add(dis);
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }
    }
}