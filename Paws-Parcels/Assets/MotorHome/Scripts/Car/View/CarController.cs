using MotorHome.Scripts.Car.Logic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace MotorHome.Scripts.Car.View
{
    [RequireComponent(typeof(CarInput))]
    public class CarController : MonoBehaviour
    {
        //private InputActionAsset m_InputActionAsset;
        [SerializeField] private string m_SteerActionName = "Steer";
        [SerializeField] private string m_ThrottleActionName = "Throttle";
        [SerializeField] private string m_HandbrakeActionName = "Handbrake";
        
        private InputAction m_SteerAction;
        private InputAction m_ThrottleAction;
        private InputAction m_HandbrakeAction;
        
        private CarInput m_CurrentCarInput;
        private GasSystem _gasSystem;

        [Inject]
        private void Construct(GasSystem gasSystem)
        {
            //m_InputActionAsset  = inputActionAsset;
            _gasSystem = gasSystem;
        }

        private void Awake()
        {
            m_CurrentCarInput = GetComponent<CarInput>();
        }
        
        private void Start()
        {
            /*if (m_InputActionAsset == null)
            {
                Debug.LogError($"[{nameof(CarController)}] No InputActionAsset assigned. Assign the reference in edit mode.");
                return;
            }

            m_SteerAction = m_InputActionAsset.FindAction(m_SteerActionName);

            if (m_SteerAction == null)
            {
                Debug.LogWarning($"[{nameof(CarController)}] '{m_SteerActionName}' action not found. Check the InputActionAsset.");
            }

            m_ThrottleAction = m_InputActionAsset.FindAction(m_ThrottleActionName);

            if (m_ThrottleAction == null)
            {
                Debug.LogWarning($"[{nameof(CarController)}] '{m_ThrottleActionName}' action not found. Check the InputActionAsset.");
            }

            m_HandbrakeAction = m_InputActionAsset.FindAction(m_HandbrakeActionName);

            if (m_HandbrakeAction == null)
            {
                Debug.LogWarning($"[{nameof(CarController)}] '{m_HandbrakeActionName}' action not found. Check the InputActionAsset.");
            }
            
            m_InputActionAsset.Enable();*/
        }


        private void Update()
        {
            if (m_CurrentCarInput == null) return;
            if (_gasSystem.GasValue == 0) return;
            
            //print(m_SteerAction?.ReadValue<float>() ?? 0.0f);
            
            m_CurrentCarInput.Steer = m_SteerAction?.ReadValue<float>() ?? 0.0f;
            m_CurrentCarInput.Throttle = m_ThrottleAction?.ReadValue<float>() ?? 0.0f;
            m_CurrentCarInput.Handbrake = m_HandbrakeAction?.ReadValue<float>() > 0.0f;
        }
    }
}
