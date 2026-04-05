using System.Collections.Generic;
using Paws_Parcels.Extentions;
using Paws_Parcels.Scripts.Player;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using UnityEngine;
using Zenject;

namespace Car_Player
{
    public class CarItemsEjector : MonoDisposable
    {
        [SerializeField] private Transform _ejectPoint;
        [SerializeField] private float _ejectForce = 5f;
        [SerializeField] private float _ejectUpForce = 2f;

        [Inject] private GameStateProvider _gameStateProvider;

        private readonly HashSet<Item> _itemsInside = new();

        protected override void OnEnable()
        {
            base.OnEnable();

            _disposable.Add(_gameStateProvider.GameProxy.IsPlayerInCar.Subscribe(value =>
            {
                if (value)
                    EjectAllItems();
            }));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Item item))
                _itemsInside.Add(item);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Item item))
                _itemsInside.Remove(item);
        }

        private void EjectAllItems()
        {
            if (_itemsInside.Count == 0)
                return;

            var ejectDirection = _ejectPoint != null
                ? -_ejectPoint.forward
                : -transform.forward;

            foreach (var item in _itemsInside)
            {
                if (item == null) continue;

                var rb = item.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    item.transform.position = _ejectPoint != null ? _ejectPoint.position : transform.position;
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    var force = ejectDirection * _ejectForce + Vector3.up * _ejectUpForce;
                    rb.AddForce(force, ForceMode.VelocityChange);
                }
            }

            _itemsInside.Clear();
        }
    }
}
