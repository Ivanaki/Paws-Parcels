using System;
using System.Collections;
using StarterAssets;
using UnityEngine;

namespace MotorHome.Scripts
{
    [RequireComponent(typeof(ThirdPersonController))]
    [RequireComponent(typeof(Player))]
    public class PlayerLastPositionRememberer : MonoBehaviour
    {
        [SerializeField] private float _rememberTime = 0.5f;
        [SerializeField] private float _minimumPlayerDistance = 2f;
        private Vector3 _lastPosition;
        private ThirdPersonController _playerController;
        private Player _player;

        public Vector3 GetLastPosition() => _lastPosition;

        private void Awake()
        {
            _playerController = GetComponent<ThirdPersonController>();
            _player = GetComponent<Player>();
        }

        private void Start()
        {
            StartCoroutine(CheckCorutine());
        }

        private IEnumerator CheckCorutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(_rememberTime);
                Remember();
            }
        }
        
        private void Remember()
        {
            if (_playerController.Grounded)
            {
                var distance = Vector3.Distance(_lastPosition, _player.transform.position);
                if (distance >= _minimumPlayerDistance)
                {
                    _lastPosition = _player.transform.position;
                }
            }
        }
    }
}