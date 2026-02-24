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
        private ReadOnlyReactiveProperty<float> _masterVolume;

        private AudioSource _audioSource;
        private float _baseVolume;
        
        private IDisposable _disposable;
        
        [Inject]
        private void Construct(SettingsStateProvider settingsStateProvider)
        {
            _masterVolume = settingsStateProvider.SettingsProxy.MasterVolume;
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _baseVolume = _audioSource.volume;
        }

        private void OnEnable()
        {
            _disposable = _masterVolume.Subscribe(v =>
            {
                _audioSource.volume = _baseVolume * v;
            });
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }
    }
}