using UnityEngine;
using Zenject;

namespace Paws_Parcels.Scripts.Car_Player.Respawn
{
    public class RespawnPoint : MonoBehaviour
    {
        private RespawnService _respawnService;

        [Inject]
        public void Construct(RespawnService respawnService)
        {
            _respawnService = respawnService;
        }

        private void OnEnable()
        {
            _respawnService.RegisterPoint(this);
        }

        private void OnDisable()
        {
            _respawnService.UnregisterPoint(this);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 1.5f);
        }
#endif
    }
}
