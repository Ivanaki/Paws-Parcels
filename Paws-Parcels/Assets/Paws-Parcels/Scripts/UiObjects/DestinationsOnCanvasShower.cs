using System;
using System.Collections.Generic;
using System.Linq;
using ObservableCollections;
using R3;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Services
{
    public abstract class ShowableInUIBehaviour : MonoBehaviour
    {
        public abstract string UiTypeId { get; }
        public Transform Transform => transform;
    }
    
    /*public interface IShowableInUIObject
    {
        string UiTypeId { get; }
        Transform Transform { get; }
    }*/
    
    public class DestinationsOnCanvasShower : MonoBehaviour
    {
        private class ObjectToShowOnUi
        {
            public ShowableInUIBehaviour ShowableInUIObject { get; }
            public UiObject UiObject { get; }

            public ObjectToShowOnUi(ShowableInUIBehaviour showableInUIObject, UiObject uiObject)
            {
                ShowableInUIObject = showableInUIObject;
                UiObject = uiObject;
            }
        }
        
        //[SerializeField] private RectTransform _barier;
        //[SerializeField] private bool _hideWhenBehind = true;
        
        private ObservableHashSet<ShowableInUIBehaviour> _objectsToShow;
        private IReadOnlyDictionary<string, UiObject> _objectsPrefabsDict;

        private List<ObjectToShowOnUi> _uiObjects = new();
        private Camera _cameraMain;
        
        [Inject]
        public void Construct(CanvasUiVisualizer canvasUiVisualizer, UiObjectsDatabase uiObjectsDatabase)
        {
            _objectsToShow = canvasUiVisualizer.ObjectsToShow;
            _objectsPrefabsDict = uiObjectsDatabase.ObjectsPrefabsDict;
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

            void Add(ShowableInUIBehaviour showable)
            {
                if (_objectsPrefabsDict.TryGetValue(showable.UiTypeId, out var uiObjectPrefab))
                {
                    var instance = Instantiate(uiObjectPrefab, transform);
                    instance.Construct(showable);

                    _uiObjects.Add(new ObjectToShowOnUi(showable, instance));
                }
                else
                {
                    Debug.LogError($"Object {showable.UiTypeId} doesn't exist");
                }
            }

            void Remove(ShowableInUIBehaviour showable)
            {
                var obj =_uiObjects.FirstOrDefault(x => x.ShowableInUIObject == showable);
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
                if(objAsset.ShowableInUIObject.Transform == null) continue;
                
                var pos = objAsset.ShowableInUIObject.Transform.position;
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