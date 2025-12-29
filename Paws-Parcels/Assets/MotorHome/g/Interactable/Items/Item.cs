using UnityEngine;

namespace MotorHome.Scripts
{
    public enum ItemType
    {
        Cube,
        Sphere
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
        
        [SerializeField] private ItemType _itemType;
        
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
            IsTouchable =  true;
            SetObjectTouchable();
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
    }
}