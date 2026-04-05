using UnityEngine;
using Zenject;

namespace Paws_Parcels.Scripts.Car_Player.Respawn
{
    public interface IRespawnPoint
    {
        public Vector3 GetPosition();
        public Quaternion GetRotation();
    }
    
    public class RespawnPoint : MonoBehaviour, IRespawnPoint
    {
        [SerializeField] private Vector3 _offset = new(0, 1.45f, 0);
        
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
        public Vector3 GetPosition()
        {
            return transform.position + _offset;
        }

        public Quaternion GetRotation() => transform.rotation;
    }
}
