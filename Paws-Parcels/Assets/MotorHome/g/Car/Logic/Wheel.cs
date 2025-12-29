using UnityEngine;

namespace MotorHome.Scripts.Car.Logic
{
    public class Wheel
    {
        private readonly RaycastSpring m_Spring;
        private readonly Rigidbody m_CarRigidbody;
        private readonly Transform m_CarTransform;
        private readonly CarSettings m_Settings;

        private Vector3 m_Forward; 
        private Vector3 m_Right;

        private Vector3 m_Position;
        private Vector3 m_Velocity;
        private float m_LateralVelocity;
        private float m_LongitudinalVelocity;
        
        private float m_GripFactor;
        private float m_GripFactorDampVel;
        
        private float m_Throttle;
        private float m_Brake;
        private float m_GripMultiplier = 1.0f;
        private float m_TractionMultiplier = 1.0f; 
        
        public Wheel(RaycastSpring spring, Rigidbody carRigidbody, CarSettings carSettings)
        {
            m_Spring = spring;
            m_CarRigidbody = carRigidbody;
            m_CarTransform = carRigidbody.transform;
            m_Settings = carSettings;
        }
        
        public float SteerAngle { get; set; }

        public float Throttle
        {
            get => m_Throttle;
            set => m_Throttle = Mathf.Clamp(value, -1.0f, 1.0f);
        }

        public float Brake
        {
            get => m_Brake;
            set => m_Brake = Mathf.Clamp01(value);
        }
        
        public float GripMultiplier
        {
            get => m_GripMultiplier;
            set => m_GripMultiplier = Mathf.Clamp01(value);
        }
        
        public float TractionMultiplier
        {
            get => m_TractionMultiplier;
            set => m_TractionMultiplier = Mathf.Clamp01(value);
        }
        
        public void FixedUpdate()
        {
            m_Position = m_CarTransform.TransformPoint(m_Spring.LocalOrigin) + -m_CarTransform.up * m_Spring.CurrentLength;
            m_Velocity = m_CarRigidbody.GetPointVelocity(m_Position);
            m_LateralVelocity = Vector3.Dot(m_Velocity, m_Right);
            m_LongitudinalVelocity = Vector3.Dot(m_Velocity, m_Forward);

            Quaternion steerQuat = Quaternion.AngleAxis(SteerAngle, Vector3.up);
            m_Forward = m_CarTransform.TransformDirection(steerQuat * Vector3.forward);
            m_Right = m_CarTransform.TransformDirection(steerQuat * Vector3.right);
            
            if (!m_Spring.IsGrounded) return;

            UpdateSlopeCompensation();
            
            UpdateGrip();

            if (Mathf.Abs(m_Throttle) > 0.0f || m_Brake > 0.0f)
            {
                if (m_Brake > 0.0f)
                {
                    UpdateBrake();
                }
                else
                {
                    UpdateDrive();
                }
            }
            else
            {
                UpdateRollingResistance();
            }
        }
        
        private Vector3 GetForcePosition(float offset)
        {
            Vector3 position = m_Spring.LocalOrigin;
            position.y = m_CarRigidbody.centerOfMass.y - offset;
            position = m_CarTransform.TransformPoint(position);
            return position;
        }

        private void UpdateSlopeCompensation()
        {
            float slopeAngle = Vector3.Angle(m_CarTransform.up, Vector3.up);
            
            float compensationMultiplier = 1.0f;

            if (slopeAngle > m_Settings.FullCompensationSlopeAngle)
            {
                const float RightAngleDeg = 90.0f;
                
                float range = RightAngleDeg - m_Settings.FullCompensationSlopeAngle;
                float ratio = (slopeAngle - m_Settings.FullCompensationSlopeAngle) / range;
                compensationMultiplier = Mathf.Clamp01(1.0f - ratio);
            }

            float slopeCompensationAccel = -Vector3.Dot(m_Right, Physics.gravity) * compensationMultiplier * GetGroundDynamicFriction() / WheelIndex.Count;
            m_CarRigidbody.AddForceAtPosition(m_Right * slopeCompensationAccel, GetForcePosition(m_Settings.GripForcesOffset), ForceMode.Acceleration);
        }
        
        private float GetGroundDynamicFriction()
        {
            return m_Spring.GroundMaterial ? m_Spring.GroundMaterial.dynamicFriction : 1.0f;
        }
        
        private float GetGroundStaticFriction()
        {
            return m_Spring.GroundMaterial ? m_Spring.GroundMaterial.staticFriction : 1.0f;
        }
        
        private void UpdateGrip()
        {
            float speedNormalized = Mathf.Clamp01(m_Velocity.magnitude / m_Settings.MaxSpeed);

            float gripFactorTarget = m_Settings.GetGrip(speedNormalized) * GetGroundDynamicFriction() * m_GripMultiplier;

            m_GripFactor = Mathf.SmoothDamp(m_GripFactor, gripFactorTarget, ref m_GripFactorDampVel, 
                m_Settings.GripSmoothTime, float.PositiveInfinity, Time.fixedDeltaTime);
            
            Vector3 slipDirection = (m_Right * m_LateralVelocity).normalized;
            
            float slopeModifier = 1.0f - Mathf.Clamp01(Vector3.Dot(slipDirection, Vector3.down));

            float velocityChange = -m_LateralVelocity * m_GripFactor * slopeModifier;
            float acceleration = velocityChange / WheelIndex.Count / Time.fixedDeltaTime;

            m_CarRigidbody.AddForceAtPosition(m_Right * acceleration, GetForcePosition(m_Settings.GripForcesOffset), ForceMode.Acceleration);
        }

        private bool CheckMaxSpeed(bool positiveThrottle)
        {
            bool isMovingForward = m_LongitudinalVelocity > 0.0f;
            float velocityMagnitude = m_Velocity.magnitude;

            if (positiveThrottle && isMovingForward && velocityMagnitude > m_Settings.MaxSpeed)
            {
                return true;
            }

            if (!positiveThrottle && !isMovingForward && velocityMagnitude > m_Settings.ReverseMaxSpeed)
            {
                return true;
            }

            return false;
        }

        private void UpdateDrive()
        {
            bool positiveThrottle = m_Throttle > 0.0f;
            
            if (Mathf.Abs(m_Throttle) > 0.0f && !CheckMaxSpeed(positiveThrottle))
            {
                Vector3 driveDirection = (m_Forward * m_Throttle).normalized;
                float driveDownProjection = Vector3.Dot(driveDirection, Vector3.down);
                
                float driveSlopeModifier = 1.0f;
                if (driveDownProjection < 0.0f)
                {
                    driveSlopeModifier += driveDownProjection; 
                }
                
                float driveStrength = positiveThrottle ? m_Settings.DriveStrength : m_Settings.ReverseDriveStrength;
                Vector3 engineForce = m_Throttle * driveStrength * m_TractionMultiplier * driveSlopeModifier * m_Forward;

                engineForce *= GetGroundStaticFriction();
                
                m_CarRigidbody.AddForceAtPosition(engineForce, GetForcePosition(m_Settings.DriveForcesOffset));
            }
        }

        private void UpdateBrake()
        {
            if (m_Brake > 0.0f)
            {
                Vector3 brakeDirection = -Mathf.Sign(m_LongitudinalVelocity) * m_Forward;
                Vector3 brakeForce = brakeDirection * (m_Brake * m_Settings.BrakeStrength * m_TractionMultiplier);

                brakeForce *= GetGroundStaticFriction();
                
                m_CarRigidbody.AddForceAtPosition(brakeForce, GetForcePosition(m_Settings.DriveForcesOffset));
            }
        }

        private void UpdateRollingResistance()
        {
            const float EpsilonVelocity = 0.1f;
            if (Mathf.Abs(m_LongitudinalVelocity) < EpsilonVelocity) return;
            
            Vector3 rollDirection = Mathf.Sign(m_LongitudinalVelocity) * m_Forward;
            float rollDownProjection = Vector3.Dot(rollDirection, Vector3.down);
            float slopeModifier = 1.0f - Mathf.Clamp01(rollDownProjection);
            
            float accel = -Mathf.Sign(m_LongitudinalVelocity) * m_Settings.RollingResistanceStrength * slopeModifier / WheelIndex.Count;

            m_CarRigidbody.AddForceAtPosition(m_Forward * accel, GetForcePosition(0.0f), ForceMode.Acceleration);
        }
    }
}
