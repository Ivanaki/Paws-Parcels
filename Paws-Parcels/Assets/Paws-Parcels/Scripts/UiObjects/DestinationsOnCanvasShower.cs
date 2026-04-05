using System.Collections.Generic;
using System.Linq;
using ObservableCollections;
using R3;
using UnityEngine;
using Zenject;

namespace Services
{
    public interface IShowableInUI
    {
        public string UiShowTypeId { get; }
        public Vector3 GetGlobalPositionShowablePoint();
        public bool IsDestroyed();
    }
    
    /*public interface IShowableInUIObject
    {
        string UiTypeId { get; }
        Transform Transform { get; }
    }*/
    
    [DefaultExecutionOrder(1000)]
    public class DestinationsOnCanvasShower : MonoBehaviour
    {
        private class ObjectToShowOnUi
        {
            public IShowableInUI ShowableInUIObject { get; }
            public UiObject UiObject { get; }

            public ObjectToShowOnUi(IShowableInUI showableInUIObject, UiObject uiObject)
            {
                ShowableInUIObject = showableInUIObject;
                UiObject = uiObject;
            }
        }
        
        //[SerializeField] private RectTransform _barier;
        //[SerializeField] private bool _hideWhenBehind = true;
        
        private ObservableHashSet<IShowableInUI> _objectsToShow;
        private IReadOnlyDictionary<string, UiObject> _objectsPrefabsDict;
        
        private DiContainer _container;

        private List<ObjectToShowOnUi> _uiObjects = new();
        private Camera _cameraMain;
        
        [Inject]
        public void Construct(CanvasUiVisualizer canvasUiVisualizer, UiObjectsDatabase uiObjectsDatabase,
            DiContainer container)
        {
            _objectsToShow = canvasUiVisualizer.ObjectsToShow;
            _objectsPrefabsDict = uiObjectsDatabase.ObjectsPrefabsDict;
            _container = container;
        }

        private CompositeDisposable _disposable = new();
        
        private void Start()
        {
            if (_objectsToShow == null)
            {
                Debug.LogError("DestinationsOnCanvasShower not constructed");
                return;
            }
            
            _cameraMain = Camera.main;
            
            foreach (var showableInUIObject in _objectsToShow) 
                Add(showableInUIObject);
            
            _disposable.Add(_objectsToShow.ObserveAdd().Subscribe(e => 
                Add(e.Value)));
            
            _disposable.Add(_objectsToShow.ObserveRemove().Subscribe(e => 
                Remove(e.Value)));
            
            _disposable.Add(_objectsToShow.ObserveClear().Subscribe(_ => 
                RemoveAll()));
            
            return;

            void Add(IShowableInUI showable)
            {
                if (_objectsPrefabsDict.TryGetValue(showable.UiShowTypeId, out var uiObjectPrefab))
                {
                    var instance = _container.InstantiatePrefabForComponent<UiObject>(uiObjectPrefab, transform);
                    //var instance = Instantiate(uiObjectPrefab, transform);
                    instance.Construct(showable);

                    _uiObjects.Add(new ObjectToShowOnUi(showable, instance));
                }
                else
                {
                    Debug.LogError($"Object {showable.UiShowTypeId} doesn't exist");
                }
            }

            void Remove(IShowableInUI showable)
            {
                var obj = _uiObjects.FirstOrDefault(x => x.ShowableInUIObject == showable);
                Destroy(obj.UiObject.gameObject);
                _uiObjects.Remove(obj);
            }

            void RemoveAll()
            {
                foreach (var obj in _uiObjects)
                {
                    Destroy(obj.UiObject.gameObject);
                }
                _uiObjects.Clear();
            }
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }

        private void LateUpdate()
        {
            foreach (var objAsset in _uiObjects)
            {
                if(objAsset.ShowableInUIObject == null) continue;
                if(objAsset.ShowableInUIObject.IsDestroyed()) continue;
                
                var pos = objAsset.ShowableInUIObject.GetGlobalPositionShowablePoint();
                var screenPoint = _cameraMain.WorldToScreenPoint(pos);
                
                bool visible = screenPoint.z > 0;
                objAsset.UiObject.gameObject.SetActive(visible);
                if (visible == false)
                    continue;
                
                objAsset.UiObject.RectTransform.position = screenPoint;
            }
        }
    }
}