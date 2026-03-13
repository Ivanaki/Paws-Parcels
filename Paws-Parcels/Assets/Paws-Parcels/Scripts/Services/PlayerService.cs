using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.Events;

namespace Services
{
    //прослойка, на которую подписывется игрок, чтобы не обращаться напрямую к игроку
    public class PlayerService
    {
        public class LookAtEvent : UnityEvent<Transform, float> { }
        
        public LookAtEvent OnLookAt = new LookAtEvent();
        public UnityEvent OffLookAt = new();
        
        public ReadOnlyReactiveProperty<bool> CanPlayerMove => _canPlayerMove;
        public ReadOnlyReactiveProperty<bool> CanPlayerMouseAndInteract => _canPlayerMouseAndInteract;

        private readonly ReactiveProperty<bool> _canPlayerMove = new(true);
        private readonly ReactiveProperty<bool> _canPlayerMouseAndInteract = new(true);
        
        private readonly List<object> _playerStoppersMovement = new();
        private readonly List<object> _playerStoppersMouseAndInteract = new();

        public void StartLookAt(Transform position, float rotationSpeed = 1f)
        {
            OnLookAt.Invoke(position, rotationSpeed);
        }

        public void StopLookAt()
        {
            OffLookAt.Invoke();
        }
        
        public void StopMovement(object playerStopper)
        {
            _playerStoppersMovement.Add(playerStopper);
            UpdatePlayerStop();
        }
        
        public void StopMouseAndInteract(object playerStopper)
        {
            _playerStoppersMouseAndInteract.Add(playerStopper);
            UpdatePlayerStop();
        }

        public void StartMovement(object playerStopper)
        {
            _playerStoppersMovement.Remove(playerStopper);
            UpdatePlayerStop();
        }

        public void StartMouseAndInteract(object playerStopper)
        {
            _playerStoppersMouseAndInteract.Remove(playerStopper);
            UpdatePlayerStop();
        }
        
        private void UpdatePlayerStop()
        {
            _canPlayerMove.Value = _playerStoppersMovement.Count == 0;
            _canPlayerMouseAndInteract.Value = _playerStoppersMouseAndInteract.Count == 0;
        }
    }
}