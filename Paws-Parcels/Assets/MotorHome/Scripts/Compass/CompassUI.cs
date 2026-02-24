using System;
using MotorHome.Scripts.Saves;
using Paws_Parcels.Car.Logic;
using R3;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class CompassUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _compassTransform;
        [SerializeField] private GameObject _compassPanel;
        
        private Transform _currentTarget;
        private Camera _camera;
        private Transform _playerTarget;
        private Transform _carTarget;
        
        private CarPlayerSwitcher _carPlayerSwitcher;
        private GameStateProvider _gameStateProvider;

        private bool _isActive = false;
        private IDisposable _disposable;

        [Inject]
        private void Construct(CarPlayerSwitcher carPlayerSwitcher, Player player, Car car,
            GameStateProvider gameStateProvider)
        {
            _carPlayerSwitcher = carPlayerSwitcher;
            _playerTarget = player.transform;
            _carTarget = car.transform;
            _gameStateProvider = gameStateProvider;
        }

        public void ActivateCompassUI()
        {
            _isActive = true;
            _compassPanel.SetActive(true);
        }
        
        private void DeactivateCompassUI()
        {
            _isActive = false;
            if (_compassPanel != null)
            {
                _compassPanel.SetActive(false);
            }

        }

        public void SetTarget(Transform target)
        {
            _currentTarget = target;
            if (target == null)
            {
                Debug.Log("Target is null");
            }
        }

        private void OnEnable()
        {
            _disposable = _gameStateProvider.Saved.Subscribe(_ =>
            {
                _gameStateProvider.CoreState.IsCompassActivate = _isActive;
            });

            var dis = _gameStateProvider.Updated.Subscribe(_ =>
            {
                if (_gameStateProvider.CoreState.IsCompassActivate)
                {
                    ActivateCompassUI();
                }
                else
                {
                    DeactivateCompassUI();
                }
            }); 
            _disposable = Disposable.Combine(_disposable, dis);
        }
        
        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if(!_isActive) return;
            
            if (_currentTarget != null)
            {
                var playerTarget = _carPlayerSwitcher.CurrentState.Value == CarPlayerSwitcher.CarOrPlayerState.Player
                    ? _playerTarget
                    : _carTarget;
                
                var direction = _currentTarget.position - playerTarget.position;
                direction.Normalize();
                
                var vector2 = new Vector2(direction.x, direction.z);
                
                var angle = Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg;
                angle = -angle;
                
                var angleWithCamera = angle + _camera.transform.eulerAngles.y/* - 180*/;
                
                _compassTransform.localRotation = Quaternion.Euler(0, 0, angleWithCamera);
            }
            else
            {
                _compassTransform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}