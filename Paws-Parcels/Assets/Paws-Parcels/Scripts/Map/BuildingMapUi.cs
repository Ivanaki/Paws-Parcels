using System;
using R3;
using UnityEngine;

namespace Map
{
    public class BuildingMapUi : MapObject
    {
        [SerializeField] private GameObject _markerGameObject;
        
        private IDisposable _disposable;
        
        public override void Construct(IShowableOnMap showableOnMap)
        {
            var building = showableOnMap as Building;

            _disposable = building.IsSelected.Subscribe(isSelected =>
            {
                _markerGameObject.SetActive(isSelected);
            });
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}