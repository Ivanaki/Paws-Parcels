using System;
using System.Collections;
using MotorHome.Scripts.Saves;
using R3;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    [RequireComponent(typeof(Car.Logic.Car))]
    [RequireComponent(typeof(Rigidbody))]
    public class CarApplySaves : MonoBehaviour
    {
        private readonly Vector3 Offset = new Vector3(0, 0.3f, 0);
        
        private GameStateProvider _gameStateProvider;
        private CompositeDisposable _disposable;
        private Rigidbody _rigidbody;
        private RigidbodyApplier _rigidbodyApplier;
        
        [Inject]
        public void Construct(GameStateProvider gameStateProvider, RigidbodyApplier  rigidbodyApplier)
        {
            _gameStateProvider = gameStateProvider;
            _rigidbodyApplier = rigidbodyApplier;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            _disposable = new CompositeDisposable();
            IDisposable dis;
            
            dis = _gameStateProvider.Updated.Subscribe(_ =>
            {
                _rigidbodyApplier.Apply(_rigidbody,
                    _gameStateProvider.CoreState.CarPosition.ToUnity() + Offset,
                    _gameStateProvider.CoreState.CarRotation.ToUnity(),
                    _gameStateProvider.CoreState.CarVelocity.ToUnity(),
                    _gameStateProvider.CoreState.CarAngularVelocity.ToUnity());
            }); _disposable.Add(dis);

            dis = _gameStateProvider.Saved.Subscribe(_ =>
            {
                _gameStateProvider.CoreState.CarPosition = new Vector3DTO(_rigidbody.transform.position);
                _gameStateProvider.CoreState.CarRotation = new QuaternionDTO(_rigidbody.transform.rotation);
                _gameStateProvider.CoreState.CarVelocity = new Vector3DTO(_rigidbody.linearVelocity);
                _gameStateProvider.CoreState.CarAngularVelocity = new Vector3DTO(_rigidbody.angularVelocity);
            });_disposable.Add(dis);
        }
        
        private void OnDisable()
        {
            _disposable.Dispose();
        }
    }
}