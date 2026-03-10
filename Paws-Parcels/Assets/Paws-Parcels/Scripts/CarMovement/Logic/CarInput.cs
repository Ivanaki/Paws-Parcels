using UnityEngine;

namespace Paws_Parcels.Car.Logic
{
    public class CarInput : MonoBehaviour
    {
        private float m_Steer;
        private float m_Throttle;

        public float Steer
        {
            get => m_Steer;
            set => m_Steer = Mathf.Clamp(value, -1.0f, 1.0f);
        }

        public float Throttle
        {
            get => m_Throttle;
            set => m_Throttle = Mathf.Clamp(value, -1.0f, 1.0f);
        }

        public bool Handbrake { get; set; }
    }
}
