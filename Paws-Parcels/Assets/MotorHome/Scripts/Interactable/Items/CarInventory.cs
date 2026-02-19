using System;
using System.Linq;
using R3;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace MotorHome.Scripts
{
    public class CarInventory : MonoBehaviour
    {
        [Header("PlayerInCollider objects")]
        [SerializeField] private ObjectPlaceInteractable _upPlace;
        [SerializeField] private ObjectPlaceInteractable _insidePlace;
        
        [Header("Items")]
        [SerializeField] private ItemType[] _upObjects;
        [SerializeField] private ItemType[] _insideObjects;
        
        [Header("View")]
        [SerializeField] private ShelfInventoryView _shelfInventoryView;
        [SerializeField] private ShelfInventoryView _upInventoryView;
        //[SerializeField] private UpItemChecker _upItemChecker;
        
        private CompositeDisposable _disposable;
        private PlayerPickUpper _playerPickUpper;
        private MessageGame _messageGame;
        
        private Inventory _inventoryShelf;
        private Inventory _inventoryUp;
        //private Item _upInventory;

        public bool ferstTime = true;
        public bool isTimeForHint = false;
        
        [SerializeField] private UnityEvent FertsTimeEvent;
        
        [Inject]
        public void Construct(PlayerPickUpper playerPickUpper, MessageGame messageGame)
        {
            _playerPickUpper = playerPickUpper;
            _messageGame = messageGame;
        }

        public void SetIsTime()
        {
            isTimeForHint = true;
        }
        public int GetItemNumberInside(Item item)
        {
            return _inventoryShelf.GetItemNumber(item);
        }
        
        public int GetItemNumberUp(Item item)
        {
            return _inventoryUp.GetItemNumber(item);
        }

        public void SetSavedItemsInside(Item[] items)
        {
            _inventoryShelf.SetSavedItemsInside(items);
        }
        public void SetSavedItemsUp(Item[] items)
        {
            _inventoryUp.SetSavedItemsInside(items);
        }
        
        private void Awake()
        {
            _inventoryShelf = new Inventory(_shelfInventoryView, Item.ItemPlace.CarInside);
            _inventoryUp = new Inventory(_upInventoryView, Item.ItemPlace.CarUp);
        }
        
        private void OnEnable()
        {
            _disposable = new CompositeDisposable();
            IDisposable dis;

            /*dis = _upItemChecker.OutItem.Subscribe(item =>
            {
                if (item == _upInventory)
                {
                    _upInventory = null;
                    print("Object out");
                }
            }); _disposable.Add(dis);*/
            
            dis = _insidePlace.OnInteract.Subscribe(_ =>
            {
                if (_playerPickUpper.IsPlayerWithBox)
                {
                    var item = _playerPickUpper.GetCurrentItem();
                    if(item._itemType == ItemType.Cat) return;
                    
                    if (_insideObjects.Contains(item.ItemType))
                    {
                        var did = _inventoryShelf.TryAddItem(item);
                        if (did)
                        {
                            _playerPickUpper.PutObjectToCar();
                            //EventManagerTest.ConditionPassed.Invoke(0);
                        }
                        else
                        {
                            _messageGame.DoMessage("Полка заполнена");
                        }
                    }
                    else
                    {
                        _messageGame.DoMessage("Объект слишком большой для этого места");
                    }
                }
                else
                {
                    var did = _inventoryShelf.TryGetItem(out var item);
                    if (did)
                    {
                        _playerPickUpper.TakeObjectIntoHands(item);
                        //EventManagerTest.ConditionPassed.Invoke(1);
                    }
                }
                
            }); _disposable.Add(dis);

            dis = _upPlace.OnInteract.Subscribe(_ =>
            {
                if (_playerPickUpper.IsPlayerWithBox)
                {
                    var item = _playerPickUpper.GetCurrentItem();
                    
                    if(item._itemType == ItemType.Cat) return;
                    
                    if (_upObjects.Contains(item.ItemType))
                    {
                        var did = _inventoryUp.TryAddItem(item);
                        if (did)
                        {
                            _playerPickUpper.PutObjectToCar();
                            //EventManagerTest.ConditionPassed.Invoke(0);
                            if (isTimeForHint)
                            {
                                if (ferstTime)
                                {
                                    FertsTimeEvent.Invoke();
                                    ferstTime = false;
                                    isTimeForHint = false;
                                }
                            }
                        }
                        else
                        {
                            _messageGame.DoMessage("Здесь нет места");
                        }
                        /*if (_upInventory == null)
                        {
                            _upInventory = item;
                            item.Place = Item.ItemPlace.CarUp;
                            _playerPickUpper.PutObjectToCar();
                            _upInventoryView.UpdateItem(item);
                        }
                        else
                        {
                            _messageGame.DoMessage("Здесь нет места");
                        }*/
                    }
                    else
                    {
                        _messageGame.DoMessage("Объект не подходит для этого места");
                    }
                }
                else
                {
                    var did = _inventoryUp.TryGetItem(out var item);
                    
                    if (did)
                    {
                        _playerPickUpper.TakeObjectIntoHands(item);
                        //EventManagerTest.ConditionPassed.Invoke(2);
                    }
                    /*if (_upInventory != null)
                    {
                        var item = _upInventory;
                        _playerPickUpper.TakeObjectIntoHands(item);
                        _upInventory = null;
                    }*/
                }

                
            }); _disposable.Add(dis);
        }

        
        
        
        
        private void OnDisable()
        {
            _disposable?.Dispose();
        }
    }
}