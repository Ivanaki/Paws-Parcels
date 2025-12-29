using MotorHome.Scripts.Car.Logic;
using UnityEngine;

namespace MotorHome.Scripts.Car.View
{
    [RequireComponent(typeof(CarSettings),  typeof(CarViewState), typeof(Logic.Car))]
    public class ViewStateLinker : MonoBehaviour
    {
        private CarViewState m_CarViewState;
        private Logic.Car _car;

        private void Awake()
        {
            _car = GetComponent<Logic.Car>();
            m_CarViewState = GetComponent<CarViewState>();
            m_CarViewState.MaxSpeed = GetComponent<CarSettings>().MaxSpeed;
            m_CarViewState.SetAllSpringsMaxLength(GetComponent<CarSettings>().SpringMaxLength);
        }
        
        private void FixedUpdate()
        {
            m_CarViewState.Forward = _car.transform.forward;
            m_CarViewState.LinearVelocity = _car.GetComponent<Rigidbody>().linearVelocity;
            m_CarViewState.ThrottleInput = _car.GetComponent<CarInput>().Throttle;
            m_CarViewState.HandbrakeInput = _car.GetComponent<CarInput>().Handbrake;

            for (int wheel = 0; wheel < WheelIndex.Count; wheel++)
            {
                m_CarViewState.Springs[wheel].Length = _car.GetSpringLength(wheel);
                m_CarViewState.Springs[wheel].Velocity = _car.GetSpringVelocity(wheel);
                    
                m_CarViewState.Wheels[wheel].SteerAngle = _car.GetWheelSteerAngle(wheel);
                m_CarViewState.Wheels[wheel].GroundMaterial = _car.GetGroundPhysicsMaterial(wheel);
            }
            
        }
    }
}
