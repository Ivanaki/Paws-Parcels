using System;
using R3;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    //[RequireComponent(typeof(ThirdPersonController))]
    [RequireComponent(typeof(Player))]
    public class PlayerLastPositionRememberer : MonoBehaviour
    {
        //[SerializeField] private float _rememberTime = 0.5f;
        //[SerializeField] private float _minimumPlayerDistance = 2f;
        private Vector3 _lastPosition;
        //private ThirdPersonController _playerController;
        private Player _player;

        [Inject] private CarPlayerSwitcher _carPlayerSwitcher;
        
        public Vector3 GetLastPosition() => _lastPosition;

        private IDisposable _disposable;
        
        private void OnEnable()
        {
            _disposable = _carPlayerSwitcher.CurrentState.Subscribe(v =>
            {
                if (v == CarPlayerSwitcher.CarOrPlayerState.Player)
                {
                    _lastPosition = _carPlayerSwitcher._playerSpawnTransform.position;
                }
            });
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }

        private void Awake()
        {
            //_playerController = GetComponent<ThirdPersonController>();
            _player = GetComponent<Player>();
        }

        private void Start()
        {
            //InvokeRepeating(nameof(Remember), _rememberTime, _rememberTime);
            //StartCoroutine(CheckCorutine());
        }
        
        private void Remember()
        {
            /*if (_playerController.Grounded && _carPlayerSwitcher.CurrentState.Value == CarPlayerSwitcher.CarOrPlayerState.Player)
            {
                var distance = Vector3.Distance(_lastPosition, _player.transform.position);
                if (distance >= _minimumPlayerDistance)
                {
                    _lastPosition = _player.transform.position;
                }
            }*/
        }
    }
}