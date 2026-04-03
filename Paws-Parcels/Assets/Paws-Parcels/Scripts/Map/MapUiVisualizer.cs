using ObservableCollections;
using R3;
using UnityEngine;

namespace Map
{
    public interface IShowableOnMap
    {
        public string MapShowTypeId { get; }
        public Vector3 GetGlobalPositionShowablePoint();
        public Quaternion GetGlobalRotationShowablePoint();
        public bool IsDestroyed();
    }
    
    public class MapUiVisualizer
    {
        public ObservableHashSet<IShowableOnMap> ObjectsToShow => _objectsToShow;
        
        private readonly ObservableHashSet<IShowableOnMap> _objectsToShow = new();
        
        public void Show(IShowableOnMap obj)
        {
            _objectsToShow.Add(obj);
        }

        public void Hide(IShowableOnMap obj)
        {
            _objectsToShow.Remove(obj);
        }
    }

    public class MapSetMarked
    {
        public readonly Subject<string> Marked = new();
    }
}