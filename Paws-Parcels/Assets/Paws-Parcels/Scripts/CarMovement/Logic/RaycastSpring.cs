using UnityEngine;

namespace Paws_Parcels.Car.Logic
{
    public class RaycastSpring
    {
        private readonly int m_WheelIndex; // FrontLeft = 0, FrontRight = 1, BackRight = 2, BackLeft = 3
        private readonly Vector3 m_LocalOrigin;
        private readonly Rigidbody m_CarRigidbody;
        private readonly Transform m_CarTransform;
        private readonly CarSettings m_Settings;
        
        private float m_CurrentLength;
        private float m_Velocity;
        private PhysicsMaterial m_GroundMaterial;
        
        public RaycastSpring(int wheelIndex, Vector3 localOrigin, Rigidbody carRigidbody, CarSettings settings)
        {
            m_WheelIndex = wheelIndex;
            m_LocalOrigin = localOrigin;
            m_CarRigidbody = carRigidbody;
            m_CarTransform = carRigidbody.transform;
            m_Settings = settings;
        }
        
        public int WheelIndex => m_WheelIndex;
        
        public Vector3 LocalOrigin => m_LocalOrigin;
        
        public float CurrentLength => m_CurrentLength;
        
        public float Velocity => m_Velocity;
        
        public bool IsGrounded => m_CurrentLength < m_Settings.SpringMaxLength;
        
        public PhysicsMaterial GroundMaterial => m_GroundMaterial;

        public void FixedUpdate()
        {
            Vector3 origin = m_CarTransform.TransformPoint(m_LocalOrigin);

            float restLength = m_Settings.SpringRestLength;
            float maxLength = m_Settings.SpringMaxLength;

            float lastLength = m_CurrentLength;

            if (Physics.Raycast(origin, -m_CarTransform.up, out var hitInfo, maxLength))
            {
                m_CurrentLength = hitInfo.distance;

                m_GroundMaterial = hitInfo.collider.sharedMaterial ? hitInfo.collider.sharedMaterial : m_Settings.DefaultGroundMaterial;

                float netForce = 0.0f;
                
                if (m_CurrentLength <= restLength)
                {
                    float compression = 1.0f - m_CurrentLength / restLength;
                    netForce += m_Settings.SpringConstant * compression;
                }
                
                m_Velocity = (m_CurrentLength - lastLength) / Time.fixedDeltaTime;
                
                float dampingForce = -m_Settings.SpringDamperConstant * m_Velocity;
                netForce += dampingForce;

                var upVector = m_CarTransform.up;
                if(upVector.x < m_Settings.OffsetSpring)
                    upVector = new Vector3(0, upVector.y, upVector.z);
                if(upVector.z < m_Settings.OffsetSpring)
                    upVector = new Vector3(upVector.x, upVector.y, 0);
                m_CarRigidbody.AddForceAtPosition(upVector * netForce, origin);
            }
            else
            {
                m_CurrentLength = maxLength;
                m_Velocity = 0.0f;

                m_GroundMaterial = null;
            }
        }
    }
}
