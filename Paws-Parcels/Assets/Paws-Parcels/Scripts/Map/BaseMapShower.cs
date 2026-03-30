using UnityEngine;
using Zenject;

namespace Map
{
    public abstract class BaseMapShower : MonoBehaviour, IShowableOnMap
    {
        [Inject] private MapUiVisualizer _mapUiVisualizer;
        
        private void OnEnable()
        {
            _mapUiVisualizer.Show(this);
        }

        private void OnDisable()
        {
            _mapUiVisualizer.Hide(this);
        }

        public abstract string MapShowTypeId { get; }
        
        public Vector3 GetGlobalPositionShowablePoint() => transform.position;

        public Quaternion GetGlobalRotationShowablePoint() => transform.rotation;

        public bool IsDestroyed() => this == null;
    }
}