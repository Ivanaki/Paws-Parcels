using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class UiObjectsDatabase
    {
        public IReadOnlyDictionary<string, UiObject> ObjectsPrefabsDict => _objectsPrefabsDict;
        private Dictionary<string, UiObject> _objectsPrefabsDict = new();
        
        public UiObjectsDatabase()
        {
            var uiObjects = Resources.LoadAll<UiObject>("UiObjects");
            
            if(uiObjects.Length == 0) return;
            
            foreach (var obj in uiObjects)
            {
                if (obj == null)
                {
                    continue;
                }
                
                if (string.IsNullOrEmpty(obj.UiTypeId))
                {
                    Debug.LogError($"object has empty UiTypeId");
                    continue;
                }

                var result = _objectsPrefabsDict.TryAdd(obj.UiTypeId, obj);
                
                if (result == false)
                { 
                    Debug.LogError("Same id in 2 quests. Need fix it. This quest dont added with id: " + obj.UiTypeId);
                }
            }
        }
    }
}