using R3;
using UnityEngine;

namespace MotorHome.Scripts
{
    [RequireComponent(typeof(Collider))]
    public class PlayerInCollider : MonoBehaviour
    {
        public BehaviorSubject<bool> IsPlayerInCollider = new(false);
        public float ColliderRadius => _sphereCollider.radius;
        
        private SphereCollider _sphereCollider;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
            
            if (TryGetComponent(out SphereCollider sphereCollider))
                if (sphereCollider.center != Vector3.zero)
                    Debug.LogError("Collider center mismatch");
            
            if (TryGetComponent(out BoxCollider boxCollider))
                if (boxCollider.center != Vector3.zero)
                    Debug.LogError("Collider center mismatch");
            
            TryGetComponent(out _sphereCollider);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                IsPlayerInCollider.OnNext(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                IsPlayerInCollider.OnNext(false);
            }
        }
    }
}