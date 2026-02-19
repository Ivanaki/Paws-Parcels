using System;
using UnityEngine;

namespace MotorHome.Scripts
{
    [Serializable]
    public class QuaternionDTO
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public QuaternionDTO()
        {
        }
        
        public QuaternionDTO(Quaternion q)
        {
            x = q.x;
            y = q.y;
            z = q.z;
            w = q.w;
        }

        public Quaternion ToUnity() => new Quaternion(x, y, z, w);
    }

}