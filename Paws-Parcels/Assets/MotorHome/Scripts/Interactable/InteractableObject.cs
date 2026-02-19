using System;
using R3;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public abstract class InteractableObject : MonoBehaviour
    {
        [SerializeField] private PlayerInCollider _playerInCollider;

        public Transform PosOverlapsTransform => _playerInCollider.transform;
        public float Radius => _playerInCollider.ColliderRadius;
        
        protected CompositeDisposable _disposable;
        private PlayerInteractable _playerInteractable;
        
        [Inject]
        private void Construct(PlayerInteractable playerInteractable)
        {
            _playerInteractable = playerInteractable;
        }
        
        protected virtual void Awake()
        {
            if (_playerInCollider == null)
            {
                _playerInCollider = GetComponent<PlayerInCollider>();
                if (_playerInCollider == null)
                    throw new NullReferenceException("PlayerInCollider not found");
            }
        }
        
        protected virtual void OnEnable()
        {
            _disposable = new CompositeDisposable();
            IDisposable dis;
            
            dis = _playerInCollider.IsPlayerInCollider.Subscribe(value =>
            {
                //print("Player in collider is " + value);
                if (value)
                    _playerInteractable.AddInteractable(this);
                else
                    _playerInteractable.TryDelInteractable(this);
            });
            _disposable.Add(dis);
        }
        
        public abstract void Interact();

        protected virtual void OnDisable()
        {
            _playerInteractable.TryDelInteractable(this);
            _disposable?.Dispose();
        }
    }
}