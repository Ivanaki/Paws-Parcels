using Paws_Parcels.Scripts.Saves.States;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Services;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Paws_Parcels.Car
{
    public class CameraSystem : MonoBehaviour
    {
        //[SerializeField] private InputActionReference m_LookAction;     // delta мыши
        //[SerializeField] private InputActionReference m_RotateButton;   // ПКМ

        [SerializeField] private float m_MouseSensitivity = 0.01f;
        [SerializeField] private float m_ReturnSpeed = 4f;

        private float m_CurrentYaw;
        
        [SerializeField] 
        private CinemachineCamera m_CinemachineCamera;

        [SerializeField] 
        private Vector3 m_CameraRegularCarOffset;

        [SerializeField] 
        private Vector3 m_CameraLargeCarOffset;

        /*[SerializeField]
        [Range(0.0f, 180.0f)] 
        private float m_CameraSwivelAngle = 90.0f;*/

        [SerializeField]
        [Range(0.0f, 120.0f)]
        private float m_CameraNormalFov = 60.0f;

        [SerializeField]
        [Range(0.0f, 120.0f)] 
        private float m_CameraFastFov = 90.0f;

        [SerializeField]
        private float m_CameraFastSpeed = 20.0f;

        [SerializeField]
        [Min(0.0001f)]
        private float m_CameraFovSmoothTime = 1.0f;

        [SerializeField] private Logic.Car _currentCar;
        
        [SerializeField] private Transform m_CameraTrackingTarget;
        private InputAction m_CameraSwivelAction;
        private Vector3 m_CameraTrackingTargetPrevPosition;
        private float m_CameraFovDampingVelocity;

        private static bool s_ControlsPanelShownAtStart;

        [Inject] private GameInputService _gameInputService;
        [Inject] private SettingsStateProvider _settingsStateProvider;

        private void LateUpdate()
        {
            if (!m_CameraTrackingTarget) return;

            //bool rotating = m_RotateButton.action.IsPressed();
            bool rotating = _gameInputService.InputSystem.CarNotMovement.LookButton.IsPressed();

            if (rotating)
            {
                Vector2 mouse = _gameInputService.InputSystem.CarNotMovement.Look.ReadValue<Vector2>();

                m_CurrentYaw += mouse.x * m_MouseSensitivity * _settingsStateProvider.SettingsProxy.CarSensitivity.CurrentValue;
            }
            else
            {
                m_CurrentYaw = Mathf.LerpAngle(m_CurrentYaw, 0f, m_ReturnSpeed * Time.deltaTime);
            }

            m_CurrentYaw = Mathf.Repeat(m_CurrentYaw, 360f);
            
            m_CameraTrackingTarget.localRotation = Quaternion.Euler(0f, m_CurrentYaw, 0f);

            UpdateFov();
        }
        
        private void UpdateFov()
        {
            float trackingTargetSpeed =
                (m_CameraTrackingTarget.position - m_CameraTrackingTargetPrevPosition).magnitude / Time.deltaTime;

            m_CameraTrackingTargetPrevPosition = m_CameraTrackingTarget.position;

            m_CinemachineCamera.Lens.FieldOfView =
                Mathf.SmoothDamp(
                    m_CinemachineCamera.Lens.FieldOfView,
                    trackingTargetSpeed > m_CameraFastSpeed ? m_CameraFastFov : m_CameraNormalFov,
                    ref m_CameraFovDampingVelocity,
                    m_CameraFovSmoothTime
                );
        }
    }
}