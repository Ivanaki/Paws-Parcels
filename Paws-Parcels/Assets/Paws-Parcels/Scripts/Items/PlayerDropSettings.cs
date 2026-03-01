using System;
using UnityEngine;
using Zenject;

namespace Items
{
    public class PlayerDropSettings : MonoBehaviour
    {
        public Transform DropTransform => _dropTransform;
        public Vector3 LocalDropLinearVelocityVector => _localDropLinearVelocityVector;
        public Vector3 LocalDropAngularVelocity => _localDropAngularVelocity;
        
        [SerializeField] private Transform _dropTransform;
        [SerializeField] private Vector3 _localDropLinearVelocityVector;
        [SerializeField] private Vector3 _localDropAngularVelocity;
        
        [Inject] private DropSettings _dropSettings;

        private void OnEnable()
        {
            _dropSettings.SetPlayerDropSettings(this);
        }

        private void OnDisable()
        {
            _dropSettings.ClearPlayerDropSettings(this);
        }
    }
}