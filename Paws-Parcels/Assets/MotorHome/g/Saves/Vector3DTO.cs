using System;
using UnityEngine;

namespace MotorHome.Scripts
{
    [Serializable]
    public class Vector3DTO {
        public float x;
        public float y;
        public float z;

        public Vector3DTO()
        {
            
        }
        
        public Vector3DTO(Vector3 v) {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public Vector3 ToUnity() => new(x, y, z);
    }
}