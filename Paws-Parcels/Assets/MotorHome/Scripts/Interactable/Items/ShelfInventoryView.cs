using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MotorHome.Scripts
{
    public class ShelfInventoryView : MonoBehaviour
    {
        [SerializeField] private Transform[] _objectsPositions;
        [field: SerializeField] public int MaxItems { private set; get; } = 10;

        [SerializeField] private UnityEvent _itemsUpdated;
        
        public void UpdateItems(Queue<Item> items)
        {
            var i = 0;
            foreach (var item in items)
            {
                item.SetObjectDontTouchable();
                item.transform.SetParent(_objectsPositions[i]);
                item.transform.localPosition = item._localPosInCar;
                item.transform.localRotation = item._localRotInCar;
                i++;
            }
            
            _itemsUpdated.Invoke();
        }
    }
}