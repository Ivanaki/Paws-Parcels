using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class MapObjectsDatabase
    {
        public IReadOnlyDictionary<string, MapObject> ObjectsPrefabsDict => _objectsPrefabsDict;
        private readonly Dictionary<string, MapObject> _objectsPrefabsDict = new();

        public MapObjectsDatabase()
        {
            var mapObjects = Resources.LoadAll<MapObject>("MapObjects");

            if (mapObjects.Length == 0)
                return;

            foreach (var obj in mapObjects)
            {
                if (obj == null)
                    continue;

                if (string.IsNullOrEmpty(obj.MapTypeId))
                {
                    Debug.LogError("MapObject has empty MapTypeId");
                    continue;
                }

                var result = _objectsPrefabsDict.TryAdd(obj.MapTypeId, obj);

                if (result == false)
                {
                    Debug.LogError($"Duplicate MapTypeId: {obj.MapTypeId}");
                }
            }
        }
    }
}