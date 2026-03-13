using NaughtyAttributes;
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
        
        [Button]
        public void TakeInfoFromTransform()
        {
            _localPosition = transform.localPosition;
            _localRotation = transform.localRotation;
        }

        [Button]
        public void ApplyInfo()
        {
            transform.localPosition = _localPosition;
            transform.localRotation = _localRotation;
        }
    }
}