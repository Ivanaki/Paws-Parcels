
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace MotorHome.Scripts
{
    public class PlayerPickUpper : MonoBehaviour
    {
        [Header("Pick up settings")]
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private Transform _sphereOverlapsTransform;
        [SerializeField] private float _overlapsRadius;
        [SerializeField] private Animator _animator;
        
        [Header("Transforms")]
        [SerializeField] private Transform _bigItemTransform;
        [SerializeField] private Transform _inRightHandTransform;
        [SerializeField] private Transform _dropTransform;
        
        public bool IsPlayerWithBox { get; private set; } = false;
        public ReadOnlyReactiveProperty<bool> CanTakeObject => _inCollider;
        
        private CompositeDisposable _disposable;
        //private CarPlayerSwitcher _carPlayerSwitcher;
        //private CoreUI _coreUI;
        
        private int _upperBodyLayerIndex;
        private Item _currentItem;
        //private GameStateProvider _gameStateProvider;
        
        private ReactiveProperty<bool> _inCollider = new(false);
        
        /*[Inject]
        private void Construct(GameStateProvider gameStateProvider)
        {
            _gameStateProvider =  gameStateProvider;
        }*/
        
        private void Awake()
        {
            _upperBodyLayerIndex = _animator.GetLayerIndex("UpperBody");
            _animator.SetLayerWeight(_upperBodyLayerIndex, 0);
        }
        
        private void OnEnable()
        {
            _disposable = new CompositeDisposable();
            //IDisposable dis;

            /*dis = _gameStateProvider.Updated.Subscribe(_ =>
            {
                
            }); _disposable.Add(dis);

            dis = _gameStateProvider.Saved.Subscribe(_ =>
            {
                _gameStateProvider.CoreState.ItemInHands = new ItemData(_currentItem);
            });_disposable.Add(dis);*/
            
        }

        private void FixedUpdate()
        {
            var item = FoundMinDistanceItem();
            _inCollider.Value = item != null;
        }
        
        private void OnDisable()
        {
            _disposable?.Dispose();
        }

        public void SetSavedItem(Item item)
        {
            if (item == null)
            {
                PutObjectToCar();
            }
            else
            {
                TakeObjectIntoHands(item);
            }
        }
        
        public Item GetCurrentItem() => _currentItem;
        
        public bool CanPlayerInteract()
        {
            if (_currentItem == null)
            {
                return PickUp();
            }
            else
            {
                return HandsClear();
            }
        }

        public void PutObjectToCar()
        {
            _animator.SetLayerWeight(_upperBodyLayerIndex, 0);
            IsPlayerWithBox = false;
            _currentItem = null;
        }

        public void TakeObjectIntoHands(Item item)
        {
            IsPlayerWithBox = true;
            _currentItem = item;
            _currentItem.SetObjectDontTouchable();
            _currentItem.Place = Item.ItemPlace.Hands;

            switch (_currentItem.ItemType)
            {
                case ItemType.Cube:
                    //EventManagerTest.ConditionPassed.Invoke(2);
                    _animator.SetLayerWeight(_upperBodyLayerIndex, 1);
                    _currentItem.transform.SetParent(_bigItemTransform);
                    
                    //конкретная анимация у аниматора
                    break;
                case ItemType.Sphere:
                   // EventManagerTest.ConditionPassed.Invoke(1);
                    _currentItem.transform.SetParent(_inRightHandTransform);
                    //конкретная анимация у аниматора
                    
                    break;
                case ItemType.Cat:
                  //  EventManagerTest.ConditionPassed.Invoke(1);
                    _animator.SetLayerWeight(_upperBodyLayerIndex, 1);
                    _currentItem.transform.SetParent(_bigItemTransform);
                    
                    break;
            }
            
            if (_currentItem._changeRotation)
            {
                _currentItem.transform.localRotation = _currentItem._localRotInHands;
            }
            _currentItem.transform.localPosition = _currentItem._localPosInHands;
        }
        
        private bool PickUp()
        {
            var item = FoundMinDistanceItem();
            if(item == null) return false;
            print(item.name);
            
            TakeObjectIntoHands(item);
            
            return true;
        }

        private bool HandsClear()
        {
            _animator.SetLayerWeight(_upperBodyLayerIndex, 0);
            //EventManagerTest.ConditionPassed.Invoke(0);
            IsPlayerWithBox = false;
            _currentItem.transform.SetParent(null);
            _currentItem.SetObjectTouchable();
            _currentItem.Place = Item.ItemPlace.Free;
            _currentItem.transform.position = _dropTransform.position;

            _currentItem = null;
            return true;
        }

        private HashSet<Item> _inArea = new();
        private HashSet<Item> _inAreaNew = new();
        
        private Item FoundMinDistanceItem()
        {
            foreach (var item in _inArea)
            {
                item.HideMark();
            }
            _inAreaNew.Clear();
            
            var hitColliders = Physics.OverlapSphere(_sphereOverlapsTransform.position, _overlapsRadius, _layerMask);
            
            var minDistance = float.MaxValue;
            Item minDisItem = null;
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent(out Item item))
                {
                    _inAreaNew.Add(item);
                    item.ShowMark();
                    
                    var dis = Vector3.Distance(hitCollider.transform.position, _sphereOverlapsTransform.position);
                    if (dis < minDistance)
                    {
                        minDistance = dis;
                        minDisItem = item;
                    }
                }
            }
            
            
            _inArea.Clear();
            foreach (var item in _inAreaNew)
            {
                _inArea.Add(item);
            }
            
            return minDisItem;
        }
    }
}