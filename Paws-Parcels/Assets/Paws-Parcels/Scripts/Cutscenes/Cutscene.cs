using System;
using R3;
using UnityEngine;
using UnityEngine.Playables;
using Zenject;

namespace Services
{
    [RequireComponent(typeof(PlayableDirector))]
    public class Cutscene : MonoBehaviour
    {
        public string CutsceneId => _cutsceneId;
        public bool HidePlayer => _hidePlayer;
        
        [SerializeField] private string _cutsceneId;
        [SerializeField] private bool _hidePlayer = false;
        
        private PlayableDirector _playableDirector;
        
        [Inject] private CutsceneService _cutsceneService;

        private void Awake()
        {
            _playableDirector = GetComponent<PlayableDirector>();
            _playableDirector.playOnAwake = false;
        }

        private void OnEnable()
        {
            _cutsceneService.RegisterCutscene(this);
        }

        private void OnDisable()
        {
            _cutsceneService.UnregisterCutscene(this);
        }

        public Observable<Unit> Play()
        {
            var sub = Observable.Timer(TimeSpan.FromSeconds(_playableDirector.playableAsset.duration));
            _playableDirector.Play();
            return sub;
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (_playableDirector == null) Awake();
            _playableDirector.playOnAwake = false;
        }
        #endif
    }
}