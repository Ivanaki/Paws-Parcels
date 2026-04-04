using System;
using Extentions;
using Paws_Parcels.Scripts.Player;
using UnityEngine;

namespace Car_Player
{
    public class UpChildTrigger : MonoBehaviour
    {
        [SerializeField] private Transform _gridTransform;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponentMy(out Item item))
            {
                item.transform.SetParent(_gridTransform);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponentMy(out Item item))
            {
                item.transform.SetParent(null);
            }
        }
    }
}