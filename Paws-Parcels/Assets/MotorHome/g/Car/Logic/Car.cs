using System;
using UnityEditor;
using UnityEngine;

namespace MotorHome.Scripts.Car.Logic
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(CarSettings))]
    [RequireComponent(typeof(CarInput))]
    public class Car : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private bool m_DrawGizmos = true;
        [SerializeField] private bool m_DrawAckermannCircle;
#endif

        private BoxCollider m_BoxCollider;
        private Rigidbody m_Rigidbody;
        private CarSettings m_Settings;
        private CarInput m_Input;
        private RaycastSpring[] m_Springs;
        private Wheel[] m_Wheels;
        
        private void Start()
        {
            if (transform.localScale != Vector3.one)
            {
                Debug.LogError($"[{nameof(Car)}] Invalid scale!");
            }

            m_Settings = GetComponent<CarSettings>();
            
            m_Input = GetComponent<CarInput>();
            
            InitializeCollider();
            
            InitializeRigidbody();

            m_Springs = new RaycastSpring[WheelIndex.Count];
            m_Wheels = new Wheel[WheelIndex.Count];
            
            for (int i = 0; i < WheelIndex.Count; i++)
            {
                m_Springs[i] = new RaycastSpring(i, GetLocalSpringOrigin(i), m_Rigidbody, m_Settings);
                
                m_Wheels[i] = new Wheel(m_Springs[i], m_Rigidbody, m_Settings);
            }
        }

        private void InitializeCollider()
        {
            m_BoxCollider = GetComponent<BoxCollider>();
            
            if (m_BoxCollider.center != Vector3.zero)
            {
                //Debug.LogError($"[{nameof(Car)}] The main BoxCollider center should be at (0,0,0). Fix this setup in edit mode.");
            }

            if (m_BoxCollider.isTrigger)
            {
                Debug.LogError($"[{nameof(Car)}] The main BoxCollider cannot be a trigger. Automatic correction applied. Fix this setup in edit mode.");
                m_BoxCollider.isTrigger = false;
            }

            if (!m_BoxCollider.enabled)
            {
                Debug.LogError($"[{nameof(Car)}] The main BoxCollider must be enabled. Automatic correction applied. Fix this setup in edit mode.");
                m_BoxCollider.enabled = true;
            }
        }
        
        private void InitializeRigidbody()
        {
            m_Rigidbody = gameObject.GetComponent<Rigidbody>();
            
            if (!m_Rigidbody.useGravity)
            {
                Debug.LogError($"[{nameof(Car)}] Rigidbody 'useGravity' must be enabled. Automatic correction applied. Fix this setup in edit mode.");
                m_Rigidbody.useGravity = true;
            }

            if (m_Rigidbody.isKinematic)
            {
                Debug.LogError($"[{nameof(Car)}] Rigidbody 'isKinematic' must be disabled. Automatic correction applied. Fix this setup in edit mode.");
                m_Rigidbody.isKinematic = false;
            }

            if (m_Rigidbody.constraints != RigidbodyConstraints.None)
            {
                Debug.LogError($"[{nameof(Car)}] Rigidbody 'constraints' must be set to 'None'. Automatic correction applied. Fix this setup in edit mode.");
                m_Rigidbody.constraints = RigidbodyConstraints.None;
            }
        }
        
        private Vector3 GetLocalSpringOrigin(int wheelIndex)
        {
            switch (wheelIndex)
            {
                case WheelIndex.FL:
                    return new Vector3(-m_Settings.TrackWidth / 2.0f, m_Settings.AxleHeight, m_Settings.Wheelbase / 2.0f);
                case WheelIndex.FR:
                    return new Vector3(m_Settings.TrackWidth / 2.0f, m_Settings.AxleHeight, m_Settings.Wheelbase / 2.0f);
                case WheelIndex.RR:
                    return new Vector3(m_Settings.TrackWidth / 2.0f, m_Settings.AxleHeight, -m_Settings.Wheelbase / 2.0f);
                case WheelIndex.RL:
                    return new Vector3(-m_Settings.TrackWidth / 2.0f, m_Settings.AxleHeight, -m_Settings.Wheelbase / 2.0f);
                default:
                    return default;
            }
        }
        
        private void FixedUpdate()
        {
            for (int i = 0; i < WheelIndex.Count; i++)
            {
                m_Springs[i].FixedUpdate();
            }
            
            UpdateSteerInput();
    
            UpdateThrottleInput();
            
            UpdateBrakeInput();
            
            UpdateHandbrakeInput();
            
            for (int i = 0; i < WheelIndex.Count; i++)
            {
                m_Wheels[i].FixedUpdate();
            }
        }

        private void UpdateSteerInput()
        {
            float wheelbase = m_Settings.Wheelbase;
            float speedNormalized = Mathf.Clamp01(m_Rigidbody.linearVelocity.magnitude / m_Settings.MaxSpeed);
            float turnRadius = m_Settings.GetTurnRadius(speedNormalized);
            float trackWidth = m_Settings.TrackWidth;
            float steerSign = Mathf.Sign(m_Input.Steer);
    
            float frontLeftAckermannAngle = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius + steerSign * trackWidth / 2.0f)) * m_Input.Steer;
            float frontRightAckermannAngle = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius - steerSign * trackWidth / 2.0f)) * m_Input.Steer;
            m_Wheels[WheelIndex.FL].SteerAngle = frontLeftAckermannAngle;
            m_Wheels[WheelIndex.FR].SteerAngle = frontRightAckermannAngle;
            m_Wheels[WheelIndex.RR].SteerAngle = 0;
            m_Wheels[WheelIndex.RL].SteerAngle = 0;
        }

        private void UpdateThrottleInput()
        {
            m_Wheels[WheelIndex.FL].Throttle = m_Input.Throttle;
            m_Wheels[WheelIndex.FR].Throttle = m_Input.Throttle;
            m_Wheels[WheelIndex.RR].Throttle = m_Input.Throttle;
            m_Wheels[WheelIndex.RL].Throttle = m_Input.Throttle;
        }

        private void UpdateBrakeInput()
        {
            float longitudinalVelocity = Vector3.Dot(m_Rigidbody.linearVelocity, transform.forward);
            bool throttleNonZero = !Mathf.Approximately(m_Input.Throttle, 0.0f);
            const float MinBrakeSpeed = 2.0f;
            bool throttleOpposeVelocity = Math.Sign(longitudinalVelocity) != Math.Sign(m_Input.Throttle);
            
            if (throttleNonZero && Mathf.Abs(longitudinalVelocity) > MinBrakeSpeed && throttleOpposeVelocity)
            {
                float brakeInput = Mathf.Abs(m_Input.Throttle);
        
                m_Wheels[WheelIndex.FL].Brake = brakeInput;
                m_Wheels[WheelIndex.FR].Brake = brakeInput;
                m_Wheels[WheelIndex.RR].Brake = brakeInput;
                m_Wheels[WheelIndex.RL].Brake = brakeInput;
            }
            else
            {
                m_Wheels[WheelIndex.FL].Brake = 0.0f;
                m_Wheels[WheelIndex.FR].Brake = 0.0f;
                m_Wheels[WheelIndex.RR].Brake = 0.0f;
                m_Wheels[WheelIndex.RL].Brake = 0.0f;
            }
        }

        private void UpdateHandbrakeInput()
        {
            if (m_Input.Handbrake)
            {
                bool positiveThrottle = m_Input.Throttle > 0;
                
                float frontGrip = positiveThrottle ? m_Settings.HandbrakeLeadingGrip : m_Settings.HandbrakeTrailingGrip;
                float rearGrip = positiveThrottle ? m_Settings.HandbrakeTrailingGrip : m_Settings.HandbrakeLeadingGrip;
                float frontTraction = positiveThrottle ? m_Settings.HandbrakeLeadingTraction : m_Settings.HandbrakeTrailingTraction;
                float rearTraction = positiveThrottle ? m_Settings.HandbrakeTrailingTraction : m_Settings.HandbrakeLeadingTraction;

                m_Wheels[WheelIndex.FL].GripMultiplier = frontGrip;
                m_Wheels[WheelIndex.FR].GripMultiplier = frontGrip;
                m_Wheels[WheelIndex.RL].GripMultiplier = rearGrip;
                m_Wheels[WheelIndex.RR].GripMultiplier = rearGrip;

                m_Wheels[WheelIndex.FL].TractionMultiplier = frontTraction;
                m_Wheels[WheelIndex.FR].TractionMultiplier = frontTraction;
                m_Wheels[WheelIndex.RL].TractionMultiplier = rearTraction;
                m_Wheels[WheelIndex.RR].TractionMultiplier = rearTraction;
            }
            else
            {
                foreach (var wheel in m_Wheels)
                {
                    wheel.GripMultiplier = 1.0f;
                    wheel.TractionMultiplier = 1.0f;
                }
            }
        }
        
        public float GetSpringLength(int wheel)
        {
            return m_Springs[wheel].CurrentLength;
        }

        public float GetSpringVelocity(int wheel)
        {
            return m_Springs[wheel].Velocity;
        }

        public float GetWheelSteerAngle(int wheelIndex)
        {
            return m_Wheels[wheelIndex].SteerAngle;
        }

        public PhysicsMaterial GetGroundPhysicsMaterial(int wheel)
        {
            return m_Springs[wheel].GroundMaterial;
        }
        


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!m_BoxCollider)
            {
                m_BoxCollider = GetComponent<BoxCollider>();
            }
            
            if (!m_Rigidbody)
            {
                m_Rigidbody = GetComponent<Rigidbody>();
            }

            if (!m_Settings)
            {
                m_Settings = GetComponent<CarSettings>();
            }
        }

        [ContextMenu(nameof(GenerateCollidersMesh))]
        private void GenerateCollidersMesh()
        {
            Undo.IncrementCurrentGroup();
            int group = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Generate Collider Meshes");

            Collider[] colliders = GetComponentsInChildren<Collider>();

            foreach (Collider col in colliders)
            {
                GameObject mesh = ColliderMeshHelper.Create(col);
                if (mesh == null) continue;

                Undo.RegisterCreatedObjectUndo(mesh, "Create Collider Mesh");
                
                Undo.SetTransformParent(mesh.transform, col.transform, false, "Parent Collider Mesh"); 
            }

            Undo.CollapseUndoOperations(group);
        }

        private void OnDrawGizmos()
        {
            if (!m_DrawGizmos) return;

            Gizmos.matrix = transform.localToWorldMatrix;
            Handles.matrix = transform.localToWorldMatrix;

            DrawBoxCollider();
            
            if (m_Rigidbody)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(m_Rigidbody.centerOfMass, 0.1f);
            }
            
            DrawSprings();

            DrawSteeringWheels();
            
            if (m_DrawAckermannCircle)
            {
                DrawAckermannCircle();
            }
        }
        
        private void DrawBoxCollider()
        {
            Gizmos.color = Color.green;
            //Gizmos.DrawWireCube(Vector3.zero, m_BoxCollider.size);
        }

        private void DrawSprings()
        {
            float restLength = m_Settings.SpringRestLength;
            float maxLength = m_Settings.SpringMaxLength;

            for (int wheel = 0; wheel < WheelIndex.Count; wheel++)
            {
                Vector3 origin = GetLocalSpringOrigin(wheel);
                float length = Application.isPlaying ? m_Springs[wheel].CurrentLength : maxLength;

                Gizmos.color = length < restLength ? Color.green : length < maxLength ? Color.yellow : Color.red;
                Gizmos.DrawLine(origin, origin + Vector3.down * length);
                Gizmos.DrawCube(origin + Vector3.down * length, new Vector3(0.1f, 0.1f, 0.1f));

                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(origin + Vector3.down * restLength, new Vector3(0.1f, 0.1f, 0.1f));
            }
        }

        private void DrawSteeringWheels()
        {
            float wheelbase = m_Settings.Wheelbase;
            float turnRadius = m_Settings.TurnRadiusMin;
            float trackWidth = m_Settings.TrackWidth;

            Vector3 ackTriangleOrigin = new(-trackWidth / 2.0f, 0.0f, -wheelbase / 2.0f);

            float turnInput = Application.isPlaying ? m_Input.Steer : 1.0f;

            float ackAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius - (trackWidth / 2.0f))) * turnInput;
            Vector3 ackRightPos = ackTriangleOrigin + new Vector3(trackWidth, 0.0f, wheelbase);
            Quaternion ackRightQuat = Quaternion.AngleAxis(ackAngleRight, Vector3.up);
            Gizmos.color = Color.white;
            Gizmos.DrawRay(ackRightPos, ackRightQuat * Vector3.forward);

            float ackAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius + (trackWidth / 2.0f))) * turnInput;
            Vector3 ackLeftPos = ackTriangleOrigin + new Vector3(0.0f, 0.0f, wheelbase);
            Quaternion ackLeftQuat = Quaternion.AngleAxis(ackAngleLeft, Vector3.up);
            Gizmos.DrawRay(ackLeftPos, ackLeftQuat * Vector3.forward);
        }

        private void DrawAckermannCircle()
        {
            float wheelbase = m_Settings.Wheelbase;
            float turnRadius = m_Settings.TurnRadiusMin;
            float trackWidth = m_Settings.TrackWidth;

            Vector3 ackTriangleOrigin = new(-trackWidth / 2.0f, 0.0f, -wheelbase / 2.0f);

            if (!Application.isPlaying)
            {
                float turnInput = 1.0f;

                Gizmos.color = Color.black;
                Gizmos.DrawLine(ackTriangleOrigin, ackTriangleOrigin + new Vector3(0.0f, 0.0f, wheelbase));
                Gizmos.DrawLine(ackTriangleOrigin, ackTriangleOrigin + new Vector3(trackWidth, 0.0f, 0.0f));

                float ackAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius - (trackWidth / 2.0f))) *
                                      turnInput;
                Vector3 ackRightPos = ackTriangleOrigin + new Vector3(trackWidth, 0.0f, wheelbase);
                Quaternion ackRightQuat = Quaternion.AngleAxis(ackAngleRight, Vector3.up);
                Gizmos.color = Color.white;
                Vector3 rightToCircle = ackRightQuat * Vector3.right;
                rightToCircle /= rightToCircle.z;
                rightToCircle *= -wheelbase;
                Gizmos.DrawRay(ackRightPos, rightToCircle);

                float ackAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius + (trackWidth / 2.0f))) *
                                     turnInput;
                Vector3 ackLeftPos = ackTriangleOrigin + new Vector3(0.0f, 0.0f, wheelbase);
                Quaternion ackLeftQuat = Quaternion.AngleAxis(ackAngleLeft, Vector3.up);
                Vector3 leftToCircle = ackLeftQuat * Vector3.right;
                leftToCircle /= leftToCircle.z;
                leftToCircle *= -wheelbase;
                Gizmos.DrawRay(ackLeftPos, leftToCircle);
            }

            Handles.color = Color.black;
            Vector3 discOrigin = ackTriangleOrigin + new Vector3(trackWidth / 2.0f + turnRadius, 0.0f);
            Handles.DrawWireDisc(discOrigin, Vector3.up, turnRadius);

            if (!Application.isPlaying)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawRay(discOrigin, Vector3.left * (turnRadius - trackWidth / 2.0f));
                Gizmos.DrawRay(discOrigin, new Vector3(0.0f, 0.0f, turnRadius));
            }
        }
#endif
    }
}
