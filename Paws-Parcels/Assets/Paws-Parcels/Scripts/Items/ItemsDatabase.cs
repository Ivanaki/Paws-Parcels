using System.Collections.Generic;
using Paws_Parcels.Scripts.Player;
using UnityEngine;

namespace Items
{
    public class ItemsDatabase
    {
        public IReadOnlyDictionary<string, Item> ItemsTypeInWorldDict => _itemsTypeInWorldDict;
        public IReadOnlyDictionary<string, ItemInHands> ItemsTypeInHandsPrefabsDict => _itemsTypeInHandsPrefabsDict;
        public IReadOnlyDictionary<string, ItemInsideCar> ItemsTypeInsideCarPrefabsDict => _itemsTypeInsideCarPrefabsDict;
        
        private readonly Dictionary<string, Item> _itemsTypeInWorldDict = new();
        private readonly Dictionary<string, ItemInHands> _itemsTypeInHandsPrefabsDict = new();
        private readonly Dictionary<string, ItemInsideCar> _itemsTypeInsideCarPrefabsDict = new();
        
        public ItemsDatabase()
        {
            var itemsInWorld = Resources.LoadAll<Item>("Items/ItemsInWorld");
            if (itemsInWorld.Length > 0)
            {
                foreach (var item in itemsInWorld)
                {
                    if (item == null)
                    {
                        continue;
                    }
                
                    if (string.IsNullOrEmpty(item.ItemTypeId))
                    {
                        Debug.LogError($"item has empty ItemTypeId");
                        continue;
                    }

                    if (item.gameObject.activeSelf)
                    {
                        Debug.LogError(@$"item with type ""{item.ItemTypeId}"" is active in prefab. Set active to false.
                            Поставь false в Active на всех префабах в ItemsInWorld");
                        return;
                    }

                    if (item.CompareTag("Item") == false)
                    {
                        Debug.LogError(@$"item with type ""{item.ItemTypeId}"" not have tag ""Item"" on it. Change tag to ""Item"".");
                        return;
                    }

                    var result = _itemsTypeInWorldDict.TryAdd(item.ItemTypeId, item);
                
                    if (result == false)
                    { 
                        Debug.LogError("Same id in 2 items. Need fix it. This item dont added with id: " + item.ItemTypeId);
                    }
                }
            }
            
            var itemsInHands = Resources.LoadAll<ItemInHands>("Items/ItemsInHands");
            if(itemsInHands.Length > 0)
            {
                foreach (var item in itemsInHands)
                {
                    if (item == null)
                    {
                        continue;
                    }
                
                    if (string.IsNullOrEmpty(item.ItemTypeId))
                    {
                        Debug.LogError($"item has empty ItemTypeId");
                        continue;
                    }

                    var result = _itemsTypeInHandsPrefabsDict.TryAdd(item.ItemTypeId, item);
                
                    if (result == false)
                    { 
                        Debug.LogError("Same id in 2 items. Need fix it. This item dont added with id: " + item.ItemTypeId);
                    }
                }
            }
            
            var itemsInsideCar = Resources.LoadAll<ItemInsideCar>("Items/ItemsInsideCar");
            if(itemsInsideCar.Length > 0)
            {
                foreach (var item in itemsInsideCar)
                {
                    if (item == null)
                    {
                        continue;
                    }
                
                    if (string.IsNullOrEmpty(item.ItemTypeId))
                    {
                        Debug.LogError($"item has empty ItemTypeId");
                        continue;
                    }

                    var result = _itemsTypeInsideCarPrefabsDict.TryAdd(item.ItemTypeId, item);
                
                    if (result == false)
                    { 
                        Debug.LogError("Same id in 2 items. Need fix it. This item dont added with id: " + item.ItemTypeId);
                    }
                }
            }
        }
    }
}