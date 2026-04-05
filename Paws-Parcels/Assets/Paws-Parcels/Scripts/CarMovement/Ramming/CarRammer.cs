using UnityEngine;

namespace Paws_Parcels.Scripts.CarMovement.Ramming
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarRammer : MonoBehaviour
    {
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            var rammable = collision.gameObject.GetComponent<IRammable>();
            if (rammable == null)
                return;

            float impactSpeed = collision.relativeVelocity.magnitude;
            if (impactSpeed < rammable.MinImpactSpeed)
                return;

            Vector3 impactDirection = _rigidbody.linearVelocity.normalized;
            rammable.OnRammed(impactSpeed, impactDirection);
        }
    }
}
