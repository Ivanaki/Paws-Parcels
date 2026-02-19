using System;
using MotorHome.Scripts.Saves;
using R3;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace MotorHome.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class CarDoorsInteractable : InteractableObject
    {
        private static readonly int Close = Animator.StringToHash("Close");
        private static readonly int Open = Animator.StringToHash("Open");
        private static readonly int OpenInstance = Animator.StringToHash("OpenInstance");
        private static readonly int CloseInstance = Animator.StringToHash("CloseInstance");

        //[SerializeField] private PlayerInCollider _inCarCollider;
        
        [SerializeField] private GameObject _stairsActivatePanel;
        [SerializeField] private GameObject _blockActivatePanel;
        //[SerializeField] private GameObject _UIcamera;
        //[SerializeField] private EventManagerTest eventManager;
        [SerializeField] private CinemachineCamera _cinemachineCamera;
        [SerializeField] private PlayerInCollider _cameraCloseCollider;

        [SerializeField] private UnityEvent _interactEvent;
        
        private Animator _animator;
        public bool _isOpen = false;
        
        [Inject]private CarPlayerSwitcher _playerSwitcher;
        public bool isFerstTime = true;
        
        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>();
            RefreshDoors(false, true);
        }
        
        public override void Interact()
        {
            //if(_inCarCollider.IsPlayerInCollider.Value) return;
            
            _isOpen = !_isOpen;
            RefreshDoors(_isOpen, false);
            /*if (isFerstTime && eventManager.nowStoryMarkNum == 1 )
            {
                EventManagerTest.StoryMark.Invoke();
                isFerstTime = false;
            }*/
            _interactEvent.Invoke();
            
        }
        
        private void RefreshDoors(bool isOpen, bool isInstance)
        {
            if (isInstance)
                _animator.SetTrigger(isOpen ?  OpenInstance : CloseInstance);
            else
                _animator.SetTrigger(isOpen ?  Open : Close);
            SetCamera(isOpen);
            _stairsActivatePanel.SetActive(isOpen);
            _blockActivatePanel.SetActive(!isOpen);
        }

        private void SetCamera(bool isOn)
        {
            _cinemachineCamera.Priority.Value = isOn ? 100 : -100;
            //_UIcamera.GetComponent<Camera>().fieldOfView = 71.1f;
            //_UIcamera.GetComponent<Camera>().fieldOfView = Camera.main.fieldOfView;
            //EventManagerTest.CloseShowable.Invoke();
            
        }
        
        private GameStateProvider _gameStateProvider;
        
        [Inject]
        private void ConstructDoor(GameStateProvider gameStateProvider)
        {
            _gameStateProvider = gameStateProvider;
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            IDisposable dis;

            dis = _playerSwitcher.CurrentState.Subscribe(v =>
            {
                if (v == CarPlayerSwitcher.CarOrPlayerState.Car)
                {
                    SetCamera(false);
                }
            }); _disposable.Add(dis);
            
            dis = _cameraCloseCollider.IsPlayerInCollider.Subscribe(value =>
            {
                if(_isOpen)
                    SetCamera(value);
            }); _disposable.Add(dis);
            
            dis = _gameStateProvider.Updated.Subscribe(_ =>
            {
                _isOpen = _gameStateProvider.CoreState.IsDoorsInCarOpen;
                RefreshDoors(_isOpen, true);
                if(_isOpen)
                    SetCamera(_cameraCloseCollider.IsPlayerInCollider.Value);
            }); _disposable.Add(dis);

            dis = _gameStateProvider.Saved.Subscribe(_ =>
            {
                _gameStateProvider.CoreState.IsDoorsInCarOpen = _isOpen;
            });_disposable.Add(dis);
        }
    }
}