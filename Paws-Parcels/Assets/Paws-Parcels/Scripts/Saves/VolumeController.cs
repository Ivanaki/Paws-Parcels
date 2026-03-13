using System;
using Paws_Parcels.Scripts.Saves.States;
using R3;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Scripts
{
    [RequireComponent(typeof(AudioSource))]
    public class VolumeController : MonoBehaviour
    {
        [SerializeField, Range(0f,1f)] private float _volumeMultiplayer = 1f;
        
        private ReadOnlyReactiveProperty<float> _masterVolume;

        private AudioSource _audioSource;
        
        private IDisposable _disposable;
        
        [Inject]
        private void Construct(SettingsStateProvider settingsStateProvider)
        {
            _masterVolume = settingsStateProvider.SettingsProxy.MasterVolume;
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            _disposable = _masterVolume.Subscribe(v =>
            {
                _audioSource.volume = v * _volumeMultiplayer;
            });
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if(_masterVolume == null) return;
            if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
            _audioSource.volume = _masterVolume.CurrentValue * _volumeMultiplayer;
        }
        #endif
    }
}