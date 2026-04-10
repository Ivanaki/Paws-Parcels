
using System;
using MotorHome.Scripts;
using Paws_Parcels.Car;
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
        private CarGasoline _carGasoline;
        
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
            _gameProxy = gameProxy;
        }

        private void Awake()
        {
            m_CurrentCarInput = GetComponent<CarInput>();
            _carGasoline = GetComponent<CarGasoline>();
        }
        
        private void Update()
        {
            if (m_CurrentCarInput == null || _gameProxy == null) return;
            if (_gameProxy.StopCar.Value || (_carGasoline != null && _carGasoline.HasGas == false))
            {
                ResetInput();
                return;
            }
            
            //print(m_SteerAction?.ReadValue<float>() ?? 0.0f);
            
            m_CurrentCarInput.Steer = m_SteerAction?.ReadValue<float>() ?? 0.0f;
            m_CurrentCarInput.Throttle = m_ThrottleAction?.ReadValue<float>() ?? 0.0f;
            m_CurrentCarInput.Handbrake = m_HandbrakeAction?.ReadValue<float>() > 0.0f;
        }

        private void ResetInput()
        {
            m_CurrentCarInput.Steer = 0f;
            m_CurrentCarInput.Throttle = 0f;
            m_CurrentCarInput.Handbrake = false;
        }
    }
}
