using System;
using R3;
using UnityEngine;

namespace MotorHome.Scripts
{
    [RequireComponent(typeof(Collider))]
    public class ItemInCar : MonoBehaviour
    {
        [SerializeField] private Transform _posToTP;
        [SerializeField] private PlayerInCollider _playerInCollider;
        [SerializeField] private GameObject _player;
        [SerializeField] private Transform _transform;
        
        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private IDisposable _disposable;
        
        /*private void OnEnable()
        {
            _disposable = _playerInCollider.IsPlayerInCollider.Subscribe(v =>
            {
                _player.transform.SetParent(v ? _transform : null);
            });
        }*/

        private void OnDisable()
        {
            _disposable?.Dispose();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Items"))
            {
                other.transform.position = _posToTP.position;
            }
        }

        /*private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Items"))
            {
                other.transform.SetParent(null);
            }
        }*/
    }
}