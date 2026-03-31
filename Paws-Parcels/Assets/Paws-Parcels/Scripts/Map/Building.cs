using R3;
using UnityEngine;
using Zenject;

namespace Map
{
    public class Building : MonoBehaviour, IShowableOnMap
    {
        public string MapShowTypeId => _mapShowTypeId;
        public ReadOnlyReactiveProperty<bool> IsSelected => _isSelected;
        
        [SerializeField] private Transform _positionOnMap;
        [SerializeField] private string _mapShowTypeId;

        private ReactiveProperty<bool> _isSelected = new(false);

        [Inject] private MapUiVisualizer _mapUiVisualizer;
        [Inject] private BuildingsLogic _buildingsLogic;

        public void SetSelected(bool isSelected)
        {
            _isSelected.Value = isSelected;
        }
        
        public Vector3 GetGlobalPositionShowablePoint() => _positionOnMap.position;
        
        public Quaternion GetGlobalRotationShowablePoint() => _positionOnMap.rotation;
        
        public bool IsDestroyed() => this == null;
        
        private void Awake()
        {
            if (_positionOnMap == null)
                _positionOnMap = transform;
        }

        private void OnEnable()
        {
            _buildingsLogic.Register(this);
            _mapUiVisualizer.Show(this);
        }

        private void OnDisable()
        {
            _buildingsLogic.Unregister(this);
            _mapUiVisualizer.Hide(this);
        }
    }
}