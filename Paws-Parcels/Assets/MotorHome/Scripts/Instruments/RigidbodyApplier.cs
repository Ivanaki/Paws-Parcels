using UnityEngine;

namespace MotorHome.Scripts
{
    public class RigidbodyApplier : MonoBehaviour
    {
        private bool _needApply = false;
        private Rigidbody _rigidbody;
        private Vector3 _position;
        private Vector3 _linearVelocity;
        private Vector3 _angularVelocity;
        private Quaternion _rotation;
        
        public void Apply(Rigidbody rigidbody, Vector3 position, Quaternion rotation, Vector3 linearVelocity,
            Vector3 angularVelocity)
        {
            _needApply = true;
            _rigidbody = rigidbody;
            _position = position;
            _rotation = rotation;
            _linearVelocity = linearVelocity;
            _angularVelocity = angularVelocity;
        }
        
        private void FixedUpdate()
        {
            if (_needApply)
            {
                _rigidbody.transform.position = _position;
                _rigidbody.transform.rotation = _rotation;
                _rigidbody.linearVelocity = _linearVelocity;
                _rigidbody.angularVelocity = _angularVelocity;
                _needApply = false;
            }
        }
    }
}