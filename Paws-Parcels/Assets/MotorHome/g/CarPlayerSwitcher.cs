using System;
using System.Collections;
using MotorHome.Scripts.Saves;
using R3;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace MotorHome.Scripts
{
    public class CarPlayerSwitcher : MonoBehaviour
    {
        private static readonly int Sit = Animator.StringToHash("Sit");

        public enum CarOrPlayerState
        {
            Car,
            Player
        }
        
        [SerializeField] private CinemachineCamera _playerCinemachineCamera;
        [SerializeField] private CinemachineCamera _carCinemachineCamera;

        //[SerializeField] private GameObject _playerGameObject;

        [SerializeField] private PlayerInCollider _playerInCollider;
        [SerializeField] private Transform _playerSpawnTransform;

        [SerializeField] private SkinnedMeshRenderer _skinnedMeshRendererPlayer;
        [SerializeField] private SkinnedMeshRenderer _skinnedMeshRendererPlayer2;
        [SerializeField] private CharacterController _characterControllerPlayer;

        [SerializeField] private Animator _sitInCarAnimator;
        [SerializeField] private CinemachineBrain _cinemachineBrain;

        public ReactiveProperty<CarOrPlayerState> CurrentState { private set; get; } = new(CarOrPlayerState.Player);
        public ReadOnlyReactiveProperty<bool> CanGetInCar => _inCollider;
        
        private InputActionAsset _inputActionAsset;
        private GameStateProvider _gameStateProvider;
        private PlayerSpawn _playerSpawn;
        private UIActiveCoreManager _uiActiveCoreManager;

        private CompositeDisposable _disposable;
        
        private ReactiveProperty<bool> _inCollider = new(false);
        private CarOrPlayerState _previousState;
       
        
        [Inject]
        private void Construct(InputActionAsset  inputActionAsset, 
            GameStateProvider gameStateProvider, PlayerSpawn playerSpawn, UIActiveCoreManager uiActiveCoreManager)
        {
            _inputActionAsset = inputActionAsset;
            _gameStateProvider = gameStateProvider;
            _playerSpawn = playerSpawn;
            _uiActiveCoreManager = uiActiveCoreManager;
        }
        
        private void OnEnable()
        {
            _disposable = new CompositeDisposable();
            IDisposable dis;
            
            dis = _gameStateProvider.Updated.Subscribe(_ =>
            {
                _previousState = CurrentState.Value;
                CurrentState.Value = _gameStateProvider.CoreState.CarOrPlayerState;

                StartCoroutine(CameraOffer());
                if (CurrentState.Value == CarOrPlayerState.Player && _previousState == CarOrPlayerState.Car)
                {
                    ExitOutCarOnlySave();
                }
                else if(CurrentState.Value == CarOrPlayerState.Car && _previousState == CarOrPlayerState.Player)
                {
                    SitToCarOnlySave();
                }
            }); _disposable.Add(dis);

            dis = _gameStateProvider.Saved.Subscribe(_ =>
            {
                _gameStateProvider.CoreState.CarOrPlayerState = CurrentState.Value;
            });_disposable.Add(dis);
            
            dis = _playerInCollider.IsPlayerInCollider.Subscribe(value =>
            {
                if (CurrentState.Value == CarOrPlayerState.Player)
                {
                    _inCollider.Value = value;
                }
            }); _disposable.Add(dis);
        }
        
        private void Start()
        {
            _playerCinemachineCamera.Priority.Enabled = true;
            _carCinemachineCamera.Priority.Enabled = true;

            _playerCinemachineCamera.Priority.Value = 1;
            _carCinemachineCamera.Priority.Value = 0;
            
            CurrentState.Value = CarOrPlayerState.Player;
            UpdateActionsMap();
        }

        private void FixedUpdate()
        {
            if (_uiActiveCoreManager.CurrentUI.Value == UIEnum.Game)
            {
                UpdateActionsMap();
            }
            else
            {
                _inputActionAsset.FindActionMap(ActionMaps.Player).Disable();
                _inputActionAsset.FindActionMap(ActionMaps.Car).Disable();
            }
        }
        
        private void OnDisable()
        {
            _disposable?.Dispose();
            //_exitCar.performed -= ExitCar;
        }

        public bool CanInteract()
        {
            print("Interact CarPlayerSwitcher");
            if (_inCollider.CurrentValue && CurrentState.Value == CarOrPlayerState.Player)
            {
                SitToCarOnlySave();
                _sitInCarAnimator.SetTrigger(Sit);
                return true;
            }
            return false;
        }

        private void SitToCarOnlySave()
        {
            print("sit to car");
            CurrentState.Value = CarOrPlayerState.Car;
            UpdateActionsMap();
            _skinnedMeshRendererPlayer.enabled = false;
            _skinnedMeshRendererPlayer2.enabled = false;
            _characterControllerPlayer.enabled = false;
            _playerCinemachineCamera.Priority.Value = -1;
            _inCollider.Value = false;
        }

        public void ExitCar()
        {
            if (CurrentState.Value == CarOrPlayerState.Car)
            {
                ExitOutCarOnlySave();
                _sitInCarAnimator.SetTrigger(Sit);
                _playerSpawn.Spawn(_playerSpawnTransform.position);
                
            }
            else
            {
                Debug.LogError("CarPlayerSwitcher cannot exit car");
            }
        }

        private void ExitOutCarOnlySave()
        {
            //print("exit out car");
            CurrentState.Value = CarOrPlayerState.Player;
            UpdateActionsMap();
            _skinnedMeshRendererPlayer.enabled = true;
            _skinnedMeshRendererPlayer2.enabled = true;
            _characterControllerPlayer.enabled = true;
            _playerCinemachineCamera.Priority.Value = 1;
        }
        
        private void UpdateActionsMap()
        {
            if (CurrentState.Value == CarOrPlayerState.Player)
            {
                _inputActionAsset.FindActionMap(ActionMaps.Player).Enable();
                _inputActionAsset.FindActionMap(ActionMaps.Car).Disable();
            }
            else
            {
                _inputActionAsset.FindActionMap(ActionMaps.Player).Disable();
                _inputActionAsset.FindActionMap(ActionMaps.Car).Enable();
            }
        }

        private IEnumerator CameraOffer()
        {
            //var time = _cinemachineBrain.DefaultBlend.Time;
            //print("time: " + time);
            _cinemachineBrain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, 0f);
            yield return new WaitForSeconds(0.2f);
            _cinemachineBrain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, 2f);
        }
        
        
    }
}