using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Paws_Parcels.Scripts.Saves.States;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Services;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Random = UnityEngine.Random;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace Paws_Parcels.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    //[RequireComponent(typeof(PlayerInput))]
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 1f;
        [Range(0, 1)] public float LandAudioVolume = 1f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        [SerializeField] private ParticleSystem _landParticalSystem;
        [SerializeField] private ParticleSystem _runParticalSystem;
        
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        
        [SerializeField] private Animator _animator;
        private CharacterController _controller;
        private MyInput _input;
        private GameObject _mainCamera;

        //my
        private const float _threshold = 0.01f;
        public float SensivityCoof = 0.008f;

        private bool _hasAnimator = true;
        private float _settingsSensivity = 1f;

        private ReadOnlyReactiveProperty<float> _masterVolume;

        private PlayerService _playerService;
        
        /*private bool IsCurrentDeviceMouse
        {
            get
            {
                return _playerInput.currentControlScheme == "KeyboardMouse";
            }
        }*/
        
        [Inject]
        private void Construct(SettingsStateProvider settingsStateProvider, PlayerService playerService)
        {
            _masterVolume = settingsStateProvider.SettingsProxy.MasterVolume;
            _playerService = playerService;
        }

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            _cinemachineTargetPitch = CinemachineCameraTarget.transform.rotation.eulerAngles.x;
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            StopLookAt();
        }

        private CompositeDisposable _disposables;
        
        private void OnEnable()
        {
            _disposables = new();
            _disposables.Add(_playerService.CanPlayerMouseAndInteract.Subscribe(value =>
            {
                if (value)
                {
                    StopLookAt();
                }
            }));
            
            _playerService.OnLookAt.AddListener(StartLookAt);
            _playerService.OffLookAt.AddListener(StopLookAt);
            _playerService.OnFixedTimeRotateEvent.AddListener(FixedRotate);
            _playerService.StopFixedTimeRotateEvent.AddListener(StopFixedTimeRotate);
            _playerService.EndFixedTimeRotateEvent.AddListener(EndFixedTimeRotate);
        }

        private void OnDisable()
        {
            _playerService.OnLookAt.RemoveListener(StartLookAt);
            _playerService.OffLookAt.RemoveListener(StopLookAt);
            _playerService.OnFixedTimeRotateEvent.RemoveListener(FixedRotate);
            _playerService.StopFixedTimeRotateEvent.RemoveListener(StopFixedTimeRotate);
            _playerService.EndFixedTimeRotateEvent.RemoveListener(EndFixedTimeRotate);
            _disposables?.Dispose();
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            //_hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<MyInput>();
            //_playerInput = GetComponent<PlayerInput>();

            AssignAnimationIDs();
            
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            //_hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();

            if (_playerService.CanPlayerMouseAndInteract.CurrentValue == false)
            {
                UpdateLookAt();
            }

            var em = _runParticalSystem.emission;
            em.rateOverDistanceMultiplier = Grounded && _input.sprint ? 1f : 0f;
        }

        private void LateUpdate()
        {
            CameraRotation();
        }
        
        private Transform _lookTarget;
        private float _lookSpeed;
        private bool _isLookingAtTarget;

        private TweenerCore<Quaternion, Vector3, QuaternionOptions> _currentFixedRotate;

        private void FixedRotate(Transform target, float duration)
        {
            var yRot = Utils.VectorToGlobalYRotation(target.position - transform.position);
            _currentFixedRotate = transform.DORotate(new Vector3(0, yRot, 0), duration);
        }

        private void StopFixedTimeRotate()
        {
            _currentFixedRotate?.Kill(true);
        }

        private void EndFixedTimeRotate()
        {
            _currentFixedRotate?.Kill();
        }
        
        public void StartLookAt(Transform target, float rotationSpeed)
        {
            if (target == null || _playerService.CanPlayerMouseAndInteract.CurrentValue)
            {
                StopLookAt();
                return;
            }
            
            _lookTarget = target;
            _lookSpeed = rotationSpeed;
            _isLookingAtTarget = true;
        }
        
        public void StopLookAt()
        {
            _isLookingAtTarget = false;
            _lookTarget = null;
        }
        
        private void UpdateLookAt()
        {
            if (!_isLookingAtTarget || _lookTarget == null)
                return;

            Vector3 direction = _lookTarget.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                _lookSpeed * Time.deltaTime
            );
        }
        
        public void SetSensivity(float sensivity)
        {
            _settingsSensivity = sensivity;
        }

        private int _applySavesFrame = 0;
        
        public void SetSaves(PlayerSaveData saveData)
        {
            _speed = saveData.Speed;
            _verticalVelocity = saveData.Verticalvelocity;
            _targetRotation = saveData.TargetRotation;
            _cinemachineTargetYaw = saveData.Yaw;
            _cinemachineTargetPitch = saveData.Pitch;
            _animationBlend = saveData.AnimationBlend;
            
            _applySavesFrame = Time.frameCount;
        }

        
        public float GetTargetYaw() => _cinemachineTargetYaw;
        public float GetTargetPitch() => _cinemachineTargetPitch;
        public float GetSpeed() => _speed;
        public float GetVerticalVelocity() => _verticalVelocity;
        public float GetTargetRotation() => _targetRotation;
        public float GetAnimationBlend() => _animationBlend;
        
        
        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                float deltaTimeMultiplier = 1.0f;

                if(_playerService.CanPlayerMouseAndInteract.CurrentValue)
                {
                    _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * SensivityCoof * _settingsSensivity;
                    _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * SensivityCoof * _settingsSensivity;
                }
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
            
            //if(_playerService.CanPlayerMouseAndInteract.CurrentValue == false) return;
            
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            if (_applySavesFrame != Time.frameCount)
            {
                if (_input.move == Vector2.zero) targetSpeed = 0.0f;

                float currentHorizontalSpeed =
                    new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

                float speedOffset = 0.1f;
                float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

                if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                    currentHorizontalSpeed > targetSpeed + speedOffset)
                {
                    _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                        Time.deltaTime * SpeedChangeRate);

                    _speed = Mathf.Round(_speed * 1000f) / 1000f;
                }
                else
                {
                    _speed = targetSpeed;
                }



                Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

                if (_input.move != Vector2.zero)
                {
                    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                      _mainCamera.transform.eulerAngles.y;
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation,
                        ref _rotationVelocity,
                        RotationSmoothTime);

                    if (_playerService.CanPlayerMouseAndInteract.CurrentValue)
                    {
                        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                    }
                }
                
                if (_hasAnimator)
                {
                    _animator.SetFloat(_animIDSpeed, _animationBlend);
                    _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
                }
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;
            
            if(_playerService.CanPlayerMove.CurrentValue == false) return;
            
            //-------------------------------- игнорим ошибку с отключенным PlayerController
            if(_controller != null && _controller.enabled && gameObject.activeInHierarchy)
                _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                 new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        public void OnFootstep(AnimationEvent animationEvent, Vector3 position)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], position,
                        FootstepAudioVolume * _masterVolume.CurrentValue);
                }
            }
        }

        public void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.00f && LandingAudioClip != null)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center),
                    LandAudioVolume * _masterVolume.CurrentValue);
            }

            _landParticalSystem.Play();
        }
    }
}
