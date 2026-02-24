using UnityEngine;

namespace Items
{
    public class ItemInHands : MonoBehaviour
    {
        public string ItemTypeId => _itemTypeId;
        public Vector3 LocalPosition => _localPosition;
        public Quaternion LocalRotation => _localRotation;
        
        [SerializeField] private string _itemTypeId;
        [SerializeField] private Vector3 _localPosition;
        [SerializeField] private Quaternion _localRotation;
    }
}