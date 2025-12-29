using System;
using R3;
using UnityEngine;

namespace MotorHome.Scripts
{
    [RequireComponent(typeof(Collider))]
    public class UpItemChecker : MonoBehaviour
    {
        public Observable<Item> OutItem => _outItem;
        private Subject<Item> _outItem = new();
        
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Item item))
            {
                _outItem.OnNext(item);
            }
        }
    }
}