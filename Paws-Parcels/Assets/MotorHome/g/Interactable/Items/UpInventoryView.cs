using System.Collections.Generic;
using UnityEngine;

namespace MotorHome.Scripts
{
    public class UpInventoryView : MonoBehaviour
    {
        [SerializeField] private Transform _positionTransform;
        
        public void UpdateItem(Item item)
        {
            item.transform.SetParent(_positionTransform);
            item.transform.localPosition = Vector3.zero;
            item.SetObjectDontTouchable();
        }
    }
}