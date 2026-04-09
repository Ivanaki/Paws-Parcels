using System;
using System.Collections.Generic;
using Paws_Parcels.Scripts.Saves.States;
using R3;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Services.Music
{
    public abstract class RandomPlaylistMusicService : IInitializable, ITickable, IDisposable
    {
        private readonly SettingsStateProvider _settingsStateProvider;
        private readonly List<AudioClip> _tracks = new();
        private readonly List<int> _trackBag = new();

        private AudioSource _audioSource;
        private GameObject _audioSourceObject;
        private CompositeDisposable _disposables;

        protected RandomPlaylistMusicService(SettingsStateProvider settingsStateProvider)
        {
            _settingsStateProvider = settingsStateProvider;
        }

        protected abstract IReadOnlyList<AudioClip> Tracks { get; }
        protected abstract string PlayerName { get; }

        public void Initialize()
        {
            _tracks.Clear();
            _trackBag.Clear();

            if (Tracks != null)
            {
                foreach (var track in Tracks)
                {
                    if (track != null)
                    {
                        _tracks.Add(track);
                    }
                }
            }

            if (_tracks.Count == 0)
            {
                return;
            }

            CreateAudioSource();
            SubscribeVolume();
            ApplyVolume();
            PlayNextTrack();
        }

        public void Tick()
        {
            if (_audioSource == null)
            {
                return;
            }

            if (_audioSource.isPlaying == false)
            {
                PlayNextTrack();
            }
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            _disposables = null;

            if (_audioSourceObject != null)
            {
                Object.Destroy(_audioSourceObject);
                _audioSourceObject = null;
                _audioSource = null;
            }
        }

        private void CreateAudioSource()
        {
            _audioSourceObject = new GameObject(PlayerName);
            _audioSource = _audioSourceObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
            _audioSource.spatialBlend = 0f;
        }

        private void SubscribeVolume()
        {
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();

            _disposables.Add(_settingsStateProvider.SettingsProxy.MasterVolume.Subscribe(_ => ApplyVolume()));
            _disposables.Add(_settingsStateProvider.SettingsProxy.MusicVolume.Subscribe(_ => ApplyVolume()));
        }

        private void ApplyVolume()
        {
            if (_audioSource == null)
            {
                return;
            }

            _audioSource.volume = _settingsStateProvider.SettingsProxy.MasterVolume.CurrentValue *
                                  _settingsStateProvider.SettingsProxy.MusicVolume.CurrentValue;
        }

        private void PlayNextTrack()
        {
            if (_tracks.Count == 0 || _audioSource == null)
            {
                return;
            }

            if (_trackBag.Count == 0)
            {
                RefillTrackBag();
            }

            var bagIndex = UnityEngine.Random.Range(0, _trackBag.Count);
            var trackIndex = _trackBag[bagIndex];
            _trackBag.RemoveAt(bagIndex);

            _audioSource.clip = _tracks[trackIndex];
            _audioSource.Play();
        }

        private void RefillTrackBag()
        {
            _trackBag.Clear();

            for (var i = 0; i < _tracks.Count; i++)
            {
                if (_tracks.Count > 1 && _audioSource.clip == _tracks[i])
                {
                    continue;
                }

                _trackBag.Add(i);
            }

            if (_trackBag.Count > 0)
            {
                return;
            }

            for (var i = 0; i < _tracks.Count; i++)
            {
                _trackBag.Add(i);
            }
        }
    }
}
