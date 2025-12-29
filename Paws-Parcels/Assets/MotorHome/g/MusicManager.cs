using System;
using System.Collections;
using System.Linq;
using MotorHome.Scripts.Saves;
using R3;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;
using Random = UnityEngine.Random;

namespace MotorHome.Scripts
{
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioSource _audioSourceThumbs;
        [SerializeField] private AudioClip[] _audioClipsSunny;
        [SerializeField] private AudioClip[] _audioClipsRainy;
        [SerializeField] private AudioClip _thumb;
        [SerializeField] private GameObject _rain;
        [SerializeField] private Material _skyboxDay;
        [SerializeField] private Material _skyboxNight;

        [SerializeField] private PhysicsMaterial _basePM;
        [SerializeField] private PhysicsMaterial _torfPM;
        
        [SerializeField] private Collider[] _colliders;
        
        private AudioClip[] _currents;
        private Coroutine _coroutine;
        private GameStateProvider _gameStateProvider;

        [Inject]
        private void Construct(GameStateProvider  gameStateProvider)
        {
            _gameStateProvider = gameStateProvider;
        }

        private CompositeDisposable _disposable;
        private IDisposable _settingsStateProxyUpdated;
        
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
                    _audioSourceThumbs.volume = value;
                });
            }); _disposable?.Add(dis);
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }

        private bool isRainy = false;
        
        public void SetRainy()
        {
            isRainy = true;
            _rain.SetActive(true);
            _currents = _audioClipsRainy;
            StartNew();
            
            RenderSettings.skybox = _skyboxNight;
            RenderSettings.fogDensity = 0.03f;
            
            foreach (var collider1 in _colliders)
            {
                collider1.material = _torfPM;
            }
        }

        public void SetSunny()
        {
            isRainy = false;
            _rain.SetActive(false);
            _currents = _audioClipsSunny;
            StartNew();
            
            RenderSettings.skybox = _skyboxDay;
            RenderSettings.fogDensity = 0.014f;
            foreach (var collider1 in _colliders)
            {
                collider1.material = _basePM;
            }
        }

        private void StartNew()
        {
            if(_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(Corutine());
        }

        private IEnumerator Corutine()
        {
            while (true)
            {
                if (isRainy)
                {
                    _audioSourceThumbs.PlayOneShot(_thumb);
                }
                var number = Random.Range(0, _currents.Length - 1);
                _audioSource.PlayOneShot(_currents[number]);
                yield return new WaitForSeconds(_currents[number].length);
            }
        }
    }
}