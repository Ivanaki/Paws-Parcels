
using System;
using MotorHome.Scripts;
using Paws_Parcels.Car.Logic;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
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
        
        private GameProxy _gameProxy;

        [Inject]
        private void Construct(GameInputService gameInputService, GameProxy gameProxy)
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
            if (_gameProxy.StopCar.Value) return;
            //if (_gasSystem.GasValue == 0) return;
            
            //print(m_SteerAction?.ReadValue<float>() ?? 0.0f);
            
            m_CurrentCarInput.Steer = m_SteerAction?.ReadValue<float>() ?? 0.0f;
            m_CurrentCarInput.Throttle = m_ThrottleAction?.ReadValue<float>() ?? 0.0f;
            m_CurrentCarInput.Handbrake = m_HandbrakeAction?.ReadValue<float>() > 0.0f;
        }
    }
}
