
using System;
using MotorHome.Scripts;
using Paws_Parcels.Car.Logic;
using Services;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Paws_Parcels.Car.View
{
    [RequireComponent(typeof(CarInput))]
    public class CarController : MonoBehaviour
    {
        private CarInput m_CurrentCarInput;
        
        private InputAction m_SteerAction;
        private InputAction m_ThrottleAction;
        private InputAction m_HandbrakeAction;

        [Inject]
        private void Construct(GameInputService gameInputService)
        {
            m_SteerAction = gameInputService.InputSystem.Car.Steer;
            m_ThrottleAction = gameInputService.InputSystem.Car.Throttle;
            m_HandbrakeAction = gameInputService.InputSystem.Car.HandBrake;
        }

        private void Awake()
        {
            m_CurrentCarInput = GetComponent<CarInput>();
        }
        
        private void Update()
        {
            if (m_CurrentCarInput == null) return;
            //if (_gasSystem.GasValue == 0) return;
            
            //print(m_SteerAction?.ReadValue<float>() ?? 0.0f);
            
            m_CurrentCarInput.Steer = m_SteerAction?.ReadValue<float>() ?? 0.0f;
            m_CurrentCarInput.Throttle = m_ThrottleAction?.ReadValue<float>() ?? 0.0f;
            m_CurrentCarInput.Handbrake = m_HandbrakeAction?.ReadValue<float>() > 0.0f;
        }
    }
}
