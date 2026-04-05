using Paws_Parcels.Scripts.Player;
using R3;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Scripts.Car_Player.Respawn
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarRespawnChecker : MonoBehaviour
    {
        [SerializeField] private float _minHeight = -10f;

        private Rigidbody _rigidbody;
        private RespawnService _respawnService;
        private ApplyInFixUpdate _applyInFixUpdate;

        [Inject]
        public void Construct(RespawnService respawnService, ApplyInFixUpdate applyInFixUpdate)
        {
            _respawnService = respawnService;
            _applyInFixUpdate = applyInFixUpdate;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _respawnService.SetCarChecker(this);
        }

        private void Update()
        {
            if (transform.position.y < _minHeight)
            {
                _respawnService.Respawn();
            }
        }

        public void Respawn()
        {
            if (_respawnService.TryGetNearestPoint(transform.position, out var pos, out var rot))
            {
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
                transform.position = pos;
                transform.rotation = rot;
            }
        }
    }
}
