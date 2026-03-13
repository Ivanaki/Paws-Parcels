using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Paws_Parcels.Car
{
    public class CameraSystem : MonoBehaviour
    {
        [SerializeField] 
        private CinemachineCamera m_CinemachineCamera;

        [SerializeField] 
        private Vector3 m_CameraRegularCarOffset;

        [SerializeField] 
        private Vector3 m_CameraLargeCarOffset;

        [SerializeField]
        [Range(0.0f, 180.0f)] 
        private float m_CameraSwivelAngle = 90.0f;

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
        
        /*[SerializeField]
        private string m_CameraTrackingTargetName = "CameraTrackingTarget";*/

        [SerializeField] private Logic.Car _currentCar;
        
        private Transform m_CameraTrackingTarget;
        private InputAction m_CameraSwivelAction;
        private Vector3 m_CameraTrackingTargetPrevPosition;
        private float m_CameraFovDampingVelocity;

        private static bool s_ControlsPanelShownAtStart;
        
        private void Start()
        {
            RefreshCameraTrackingTarget();
        }

        private void LateUpdate()
        {
            if (m_CameraTrackingTarget)
            {
                float cameraSwivelInput = m_CameraSwivelAction?.ReadValue<float>() ?? 0.0f;
                m_CameraTrackingTarget.localEulerAngles = new Vector3(0.0f, cameraSwivelInput * m_CameraSwivelAngle, 0.0f);

                float trackingTargetSpeed = (m_CameraTrackingTarget.position - m_CameraTrackingTargetPrevPosition).magnitude / Time.deltaTime;
                m_CameraTrackingTargetPrevPosition = m_CameraTrackingTarget.position;

                m_CinemachineCamera.Lens.FieldOfView = Mathf.SmoothDamp(m_CinemachineCamera.Lens.FieldOfView,
                    trackingTargetSpeed > m_CameraFastSpeed ? m_CameraFastFov : m_CameraNormalFov,
                    ref m_CameraFovDampingVelocity,
                    m_CameraFovSmoothTime);
            }
        }

        private void RefreshCameraTrackingTarget()
        {
            /*Transform trackingTarget = null;
            for (int i = 0; i < _currentCar.transform.childCount; i++)
            {
                Transform child = _currentCar.transform.GetChild(i);

                if (child.name == m_CameraTrackingTargetName)
                {
                    trackingTarget = child;
                    break;
                }
            }

            if (trackingTarget == null)
            {
                var trackingTargetGo = new GameObject(m_CameraTrackingTargetName);
                trackingTargetGo.transform.SetParent(_currentCar.transform);
                trackingTargetGo.transform.localPosition = Vector3.zero;
                trackingTargetGo.transform.localRotation = Quaternion.identity;
                trackingTargetGo.transform.localScale = Vector3.one;

                trackingTarget = trackingTargetGo.transform;
            }*/

            /*m_CinemachineCamera.Target = new CameraTarget
            {
                TrackingTarget = trackingTarget, LookAtTarget = trackingTarget
            };

            m_CameraTrackingTarget = trackingTarget;*/

            /*var cinemachineFollow = m_CinemachineCamera.GetComponent<CinemachineFollow>();
            var carCollider = _currentCar.GetComponent<BoxCollider>();
            const float LargeCarSize = 7.0f;
            cinemachineFollow.FollowOffset = carCollider.size.magnitude < LargeCarSize ? m_CameraRegularCarOffset : m_CameraLargeCarOffset;*/
        }
    }
}