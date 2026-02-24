using System;
using UnityEngine;
using Zenject;

namespace Items
{
    public class PlayerDropSettings : MonoBehaviour
    {
        public Transform DropTransform;
        public Vector3 LocalDropLinearVelocityVector;
        public float DropLinVelocityCoof = 0f;
        public Vector3 LocalDropAngularVelocity;
        public float DropAnVelocityCoof = 0f;
        
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