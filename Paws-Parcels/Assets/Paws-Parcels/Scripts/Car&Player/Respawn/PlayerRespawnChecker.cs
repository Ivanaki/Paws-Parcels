using Paws_Parcels.Scripts.Player;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Scripts.Car_Player.Respawn
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerRespawnChecker : MonoBehaviour
    {
        [SerializeField] private float _minHeight = -10f;

        private CharacterController _characterController;
        private RespawnService _respawnService;
        private ApplyInFixUpdate _applyInFixUpdate;
        private ReadOnlyReactiveProperty<bool> _isPlayerInCar;

        [Inject]
        public void Construct(RespawnService respawnService, ApplyInFixUpdate applyInFixUpdate,
            GameStateProvider gameStateProvider)
        {
            _respawnService = respawnService;
            _applyInFixUpdate = applyInFixUpdate;
            _isPlayerInCar = gameStateProvider.GameProxy.IsPlayerInCar;
        }

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _respawnService.SetPlayerChecker(this);
        }

        private void Update()
        {
            if (_isPlayerInCar.CurrentValue)
                return;

            if (transform.position.y < _minHeight)
            {
                _respawnService.Respawn();
            }
        }

        public void Respawn()
        {
            if (_respawnService.TryGetNearestPoint(transform.position, out var pos, out _))
            {
                _applyInFixUpdate.ObserveApply().Take(1).Subscribe(_ =>
                {
                    _characterController.transform.position = pos;
                });
            }
        }
    }
}
