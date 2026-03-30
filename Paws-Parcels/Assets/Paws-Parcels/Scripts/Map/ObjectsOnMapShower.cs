using System.Collections.Generic;
using System.Linq;
using ObservableCollections;
using R3;
using UnityEngine;
using Zenject;

namespace Map
{
    public class ObjectsOnMapShower : MonoBehaviour
    {
        private class ObjectToShowOnMap
        {
            public IShowableOnMap ShowableOnMapObject { get; }
            public MapObject MapObject { get; }

            public ObjectToShowOnMap(IShowableOnMap showableOnMapObject, MapObject mapObject)
            {
                ShowableOnMapObject = showableOnMapObject;
                MapObject = mapObject;
            }
        }

        [Header("Map Rect")]
        [SerializeField] private RectTransform _mapContainer;

        [Header("World Bounds")]
        [SerializeField] private Vector2 _worldMin;
        [SerializeField] private Vector2 _worldMax;

        [Header("Settings")]
        [SerializeField] private bool _clampInsideMap = true;
        
        [Header("Rotation")]
        [SerializeField] private bool _rotateIcons = true;

        private ObservableHashSet<IShowableOnMap> _objectsToShow;
        private IReadOnlyDictionary<string, MapObject> _objectsPrefabsDict;
        private DiContainer _container;

        private readonly List<ObjectToShowOnMap> _mapObjects = new();
        private readonly CompositeDisposable _disposable = new();

        [Inject]
        public void Construct(
            MapUiVisualizer mapUiVisualizer,
            MapObjectsDatabase mapObjectsDatabase,
            DiContainer container)
        {
            _objectsToShow = mapUiVisualizer.ObjectsToShow;
            _objectsPrefabsDict = mapObjectsDatabase.ObjectsPrefabsDict;
            _container = container;
        }

        private void Start()
        {
            if (_objectsToShow == null)
            {
                Debug.LogError("ObjectsOnMapShower not constructed");
                return;
            }

            foreach (var showable in _objectsToShow)
                Add(showable);

            _disposable.Add(_objectsToShow.ObserveAdd().Subscribe(e =>
                Add(e.Value)));

            _disposable.Add(_objectsToShow.ObserveRemove().Subscribe(e =>
                Remove(e.Value)));

            _disposable.Add(_objectsToShow.ObserveClear().Subscribe(_ =>
                RemoveAll()));
        }

        private void LateUpdate()
        {
            for (int i = _mapObjects.Count - 1; i >= 0; i--)
            {
                var obj = _mapObjects[i];

                if (obj.ShowableOnMapObject == null || obj.ShowableOnMapObject.IsDestroyed())
                {
                    if (obj.MapObject != null)
                        Destroy(obj.MapObject.gameObject);

                    _mapObjects.RemoveAt(i);
                    continue;
                }

                var worldPos = obj.ShowableOnMapObject.GetGlobalPositionShowablePoint();
                var localMapPos = WorldToMapLocalPosition(worldPos);
                obj.MapObject.RectTransform.anchoredPosition = localMapPos;
                
                if (_rotateIcons)
                {
                    var worldRot = obj.ShowableOnMapObject.GetGlobalRotationShowablePoint();
                    var localMapRotZ = WorldToMapLocalRotation(worldRot);

                    obj.MapObject.RectTransform.localRotation = Quaternion.Euler(0f, 0f, localMapRotZ);
                }
            }
        }

        private Vector2 WorldToMapLocalPosition(Vector3 worldPos)
        {
            float worldWidth = _worldMax.x - _worldMin.x;
            float worldHeight = _worldMax.y - _worldMin.y;

            if (Mathf.Approximately(worldWidth, 0f) || Mathf.Approximately(worldHeight, 0f))
            {
                Debug.LogError("World bounds are invalid in ObjectsOnMapShower");
                return Vector2.zero;
            }

            float normalizedX = (worldPos.x - _worldMin.x) / worldWidth;
            float normalizedY = (worldPos.z - _worldMin.y) / worldHeight;

            if (_clampInsideMap)
            {
                normalizedX = Mathf.Clamp01(normalizedX);
                normalizedY = Mathf.Clamp01(normalizedY);
            }

            float mapWidth = _mapContainer.rect.width;
            float mapHeight = _mapContainer.rect.height;

            float localX = (normalizedX - 0.5f) * mapWidth;
            float localY = (normalizedY - 0.5f) * mapHeight;

            return new Vector2(localX, localY);
        }

        private float WorldToMapLocalRotation(Quaternion worldRot)
        {
            float worldYaw = worldRot.eulerAngles.y;
            return -worldYaw;
        }

        private void Add(IShowableOnMap showable)
        {
            if (_objectsPrefabsDict.TryGetValue(showable.MapShowTypeId, out var mapObjectPrefab))
            {
                var instance = _container.InstantiatePrefabForComponent<MapObject>(mapObjectPrefab, _mapContainer);
                instance.Construct(showable);

                _mapObjects.Add(new ObjectToShowOnMap(showable, instance));
            }
            else
            {
                Debug.LogError($"Map object with id '{showable.MapShowTypeId}' doesn't exist");
            }
        }

        private void Remove(IShowableOnMap showable)
        {
            var obj = _mapObjects.FirstOrDefault(x => x.ShowableOnMapObject == showable);
            if (obj == null)
                return;

            if (obj.MapObject != null)
                Destroy(obj.MapObject.gameObject);

            _mapObjects.Remove(obj);
        }

        private void RemoveAll()
        {
            foreach (var obj in _mapObjects)
            {
                if (obj.MapObject != null)
                    Destroy(obj.MapObject.gameObject);
            }

            _mapObjects.Clear();
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}