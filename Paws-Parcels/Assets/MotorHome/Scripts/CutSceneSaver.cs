using System;
using MotorHome.Scripts.Saves;
using R3;
using UnityEngine;
using UnityEngine.Playables;
using Zenject;

namespace MotorHome.Scripts
{
    public class CutSceneSaver : MonoBehaviour
    {
        [SerializeField] private PlayableDirector _cutScene;
        
        private GameStateProvider _gameStateProvider;
        private CompositeDisposable _disposable;
        
        [Inject]
        public void Construct(GameStateProvider gameStateProvider)
        {
            _gameStateProvider = gameStateProvider;
        }

        private void Awake()
        {
            _cutScene.gameObject.SetActive(false);
            //Invoke(nameof(CutScenePlayed), (float)_cutScene.duration);
        }

        private float _startTime;
        
        private void Start()
        {
            _startTime = Time.time;
        }

        //private void CutScenePlayed()
        //{
        //    _gameStateProvider.CoreState.IsCutSceneIsPlayed = true;
        //}
        
        private void OnEnable()
        {
            _disposable = new CompositeDisposable();
            IDisposable dis;
            
            dis = _gameStateProvider.Updated.Subscribe(_ =>
            {
                var isPlayed = _gameStateProvider.CoreState.IsCutSceneIsPlayed;
                _cutScene.gameObject.SetActive(!isPlayed);
                if (isPlayed == false)
                {
                    _cutScene.Play();
                }
            }); _disposable.Add(dis);

            dis = _gameStateProvider.Saved.Subscribe(_ =>
            {
                _gameStateProvider.CoreState.IsCutSceneIsPlayed = Time.time - _startTime > _cutScene.duration;
            });_disposable.Add(dis);
        }
        
        private void OnDisable()
        {
            _disposable.Dispose();
        }
    }
}