using Paws_Parcels.Scripts.Saves.States;
using R3;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Scripts
{
    [RequireComponent(typeof(AudioSource))]
    public class VolumeController : MonoBehaviour
    {
        [SerializeField] private bool _useMusicVolume;
        [SerializeField, Range(0f,1f)] private float _volumeMultiplayer = 1f;
        
        private ReadOnlyReactiveProperty<float> _masterVolume;
        private ReadOnlyReactiveProperty<float> _musicVolume;

        private AudioSource _audioSource;
        
        private CompositeDisposable _disposables;
        
        [Inject]
        private void Construct(SettingsStateProvider settingsStateProvider)
        {
            _masterVolume = settingsStateProvider.SettingsProxy.MasterVolume;
            _musicVolume = settingsStateProvider.SettingsProxy.MusicVolume;
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            _disposables = new CompositeDisposable();

            if (_masterVolume != null)
            {
                _disposables.Add(_masterVolume.Subscribe(_ => ApplyVolume()));
            }

            if (_useMusicVolume && _musicVolume != null)
            {
                _disposables.Add(_musicVolume.Subscribe(_ => ApplyVolume()));
            }

            ApplyVolume();
        }

        private void OnDisable()
        {
            _disposables?.Dispose();
            _disposables = null;
        }

        private void ApplyVolume()
        {
            if (_audioSource == null)
                return;

            var musicVolume = _useMusicVolume ? _musicVolume?.CurrentValue ?? 1f : 1f;
            _audioSource.volume = (_masterVolume?.CurrentValue ?? 1f) * musicVolume * _volumeMultiplayer;
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
            ApplyVolume();
        }
        #endif
    }
}
