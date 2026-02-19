using UnityEngine;

namespace MotorHome.Scripts
{
    public enum ItemType
    {
        Cube,
        Sphere,
        Cat
    }
    
    [RequireComponent(typeof(Collider))]
    public class Item : MonoBehaviour
    {
        public enum ItemPlace
        {
            Free,
            Hands,
            CarUp,
            CarInside
        }

        public GameObject _showableObject;
        
        public ItemType _itemType;

        [Header("InHands")]
        public Vector3 _localPosInHands;
        public bool _changeRotation = false;
        public Quaternion _localRotInHands = new(-0.566174269f,-0.542404592f,0.22962895f,-0.576640725f);
        
        [Header("InCar")]
        public Vector3 _localPosInCar;
        public Quaternion _localRotInCar;
        
        public ItemType ItemType => _itemType;
        [HideInInspector] public ItemPlace Place;
        public bool IsTouchable { private set; get; } = false;
        //public Observable<Unit> GetPickedUp => _playerPickedUpper;
        
        //private Subject<Unit> _playerPickedUpper = new();
        private Collider _collider;
        public Rigidbody Rigidbody { private set; get; }

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            Rigidbody = GetComponent<Rigidbody>();
            IsTouchable = true;
            _collider.enabled = true;
            HideMark();
        }
        
        public void SetObjectDontTouchable()
        {
            IsTouchable = false;
            _collider.enabled = false;
            if(Rigidbody != null)
                Rigidbody.isKinematic = true;
        }

        public void SetObjectTouchable()
        {
            IsTouchable = true;
            _collider.enabled = true;
            if(Rigidbody != null)
                Rigidbody.isKinematic = false;
        }

        public void ShowMark()
        {
            if(_showableObject != null)
                _showableObject.SetActive(true);
        }

        public void HideMark() 
        {
            if(_showableObject != null)
                _showableObject.SetActive(false);
        }
    }
}