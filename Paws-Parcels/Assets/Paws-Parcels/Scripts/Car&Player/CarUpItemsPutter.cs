using System;
using Paws_Parcels.Extentions;
using Paws_Parcels.Scripts;
using Paws_Parcels.Scripts.Player;
using R3;
using Services;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Car_Player
{
    public class CarUpItemsPutter : MonoDisposable
    {
        [SerializeField] private GameObject _hint;
        [SerializeField] private float _playerRotSpeed = 100000f;
        [SerializeField] private float _yOffsetWhenDrop = 2f;
        [SerializeField] private float _waitTimeWhenObjectFlyInPut = 1.5f;
        [SerializeField] private TriggerEvent _exitCollider;
        
        [Inject] private PutInCarItemsService _putInCarItemsService;
        [Inject] private PlayerService _playerService;
        [Inject] private CursorService _cursorService;
        [Inject] private GameInputService _gameInputService;
        
        private Camera _camera;
        private Vector3 _hintPosition;
        private Vector3 _hintNormal;
        private readonly ReactiveProperty<bool> _hintEnable = new(false);
        private LayerMask _rayLayerMask;
        private readonly ReactiveProperty<Item> _highlightedItem = new(null);
        private Transform _playerLookAtTransform;

        private void Awake()
        {
            _playerLookAtTransform = new GameObject().transform;
            _rayLayerMask = LayerMask.GetMask("CarUp", "Items");
            GetCamera();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _disposable.Add(_exitCollider.OnTriggerExitObservable.Subscribe(value =>
            {
                _putInCarItemsService.TryCancel();
            }));
            
            _disposable.Add(_hintEnable.Subscribe(value => _hint.SetActive(value)));
            _disposable.Add(_highlightedItem.Pairwise().Subscribe(itemPair =>
            {
                var (previousItem, currentItem) = itemPair;
                if(previousItem != null)
                    previousItem.UnhighlightItem(this);
                if(currentItem != null)
                    currentItem.HighlightItem(this);
            }));
            
            _disposable.Add(_putInCarItemsService.PutState.Subscribe(state =>
            {
                if (state is PutItemState.Put or PutItemState.Take)
                {
                    _playerService.StartLookAt(_playerLookAtTransform, _playerRotSpeed);
                    _cursorService.SetCursorStateLocked(false);
                }
                else if (state is PutItemState.Null)
                {
                    _playerService.StopLookAt();
                    _cursorService.SetCursorStateLocked(true);
                }
            }));
            
            _gameInputService.InputSystem.PlayerNotMovement.Attack.performed += MouseLeftClicked;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            _gameInputService.InputSystem.PlayerNotMovement.Attack.performed -= MouseLeftClicked;
        }

        private void Update()
        {
            if (_putInCarItemsService.PutState.CurrentValue == PutItemState.Put)
            {
                _highlightedItem.Value = null;
                
                if (CheckRaycast(out var hit))
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("CarUp") 
                        ||
                        hit.collider.gameObject.layer == LayerMask.NameToLayer("Items") &&
                        ColliderRedirector.TryGet(hit.collider.gameObject, out Item item) &&
                        item.IsItemInCarUp)
                    {
                        _hintEnable.Value = true;
                        _hintPosition = hit.point;
                        _hintNormal = hit.normal;
                        
                    }
                    
                    _playerLookAtTransform.position = hit.point;
                }
            }
            else if (_putInCarItemsService.PutState.CurrentValue == PutItemState.Take)
            {
                _hintEnable.Value = false;
                
                if (CheckRaycast(out var hit))
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Items") &&
                        ColliderRedirector.TryGet(hit.collider.gameObject, out Item item) &&
                        item.IsItemInCarUp)
                    {
                        item.HighlightItem(this);
                        _highlightedItem.Value = item;
                    }
                    else
                    {
                        _highlightedItem.Value = null;
                    }
                    
                    _playerLookAtTransform.position = hit.point;
                }
                else
                {
                    _highlightedItem.Value = null;
                }
            }
            else if (_putInCarItemsService.PutState.CurrentValue == PutItemState.Null)
            {
                _hintEnable.Value = false;
                _highlightedItem.Value = null;
            }

            if (_hintEnable.Value)
            {
                _hint.transform.position = _hintPosition;
                _hint.transform.up = _hintNormal;
            }
        }

        private bool CheckRaycast(out RaycastHit hit)
        {
            hit = new RaycastHit();
            
            if (_camera == null)
            {
                Debug.LogWarning("camera is null");
                return false;
            }
                
            if (Mouse.current == null) return false;
                
            var mousePos = Mouse.current.position.ReadValue();
            var ray = _camera.ScreenPointToRay(mousePos);

            return Physics.Raycast(ray, out hit, 100f, _rayLayerMask);
        }

        private void GetCamera()
        {
            _camera = Camera.main;
        }
        
        private void MouseLeftClicked(InputAction.CallbackContext obj = new())
        {
            if (_putInCarItemsService.PutState.CurrentValue == PutItemState.Put)
            {
                if (CheckRaycast(out var hit))
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("CarUp") 
                        ||
                        hit.collider.gameObject.layer == LayerMask.NameToLayer("Items") &&
                        ColliderRedirector.TryGet(hit.collider.gameObject, out Item item) &&
                        item.IsItemInCarUp)
                    {
                        _putInCarItemsService.PutPositionClicked(hit.point, _yOffsetWhenDrop, _waitTimeWhenObjectFlyInPut);
                    }
                }
            }
            else if(_putInCarItemsService.PutState.CurrentValue == PutItemState.Take)
            {
                if (CheckRaycast(out var hit))
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Items") &&
                        ColliderRedirector.TryGet(hit.collider.gameObject, out Item item) &&
                        item.IsItemInCarUp)
                    {
                        _putInCarItemsService.TakeItem(item);
                    }
                }
            }
        }
    }
}