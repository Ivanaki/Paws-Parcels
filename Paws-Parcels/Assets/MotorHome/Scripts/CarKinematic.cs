using System;
using R3;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarKinematic : MonoBehaviour
    {
        [SerializeField] private float _magnitude = 0.1f;
        
        [Inject] private CarPlayerSwitcher _carPlayerSwitcher;

        private bool _check = false;
        private IDisposable _disposable;
        
        private Rigidbody _rigidbody;
        
        private void OnEnable()
        {
            _rigidbody = GetComponent<Rigidbody>();
            
            _disposable = _carPlayerSwitcher.CurrentState.Subscribe(v =>
            {
                if (v == CarPlayerSwitcher.CarOrPlayerState.Car)
                {
                    _rigidbody.isKinematic = false;
                }
                else
                {
                    _check = true;
                }
            });
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }

        private void FixedUpdate()
        {
            if (_check)
            {
                if (_rigidbody.linearVelocity.magnitude < _magnitude)
                {
                    _check = false;
                    _rigidbody.isKinematic = true;
                }
            }
        }
    }
}