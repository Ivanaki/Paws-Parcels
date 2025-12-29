using System;
using UnityEngine;

namespace MotorHome.Scripts
{
    public class CopyCameraTransformAndSetScale : MonoBehaviour
    {
        private static float _normalDistance = 6.5f;
        
        private Vector3 _startScale;

        private void Awake()
        {
            _startScale = transform.localScale;
        }

        private void Update()
        {
            //transform.LookAt(Camera.main.transform);
            //transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, transform.eulerAngles.z);
            transform.rotation = Camera.main.transform.rotation;
            var distance = Vector3.Distance(Camera.main.transform.position, transform.position);
            //print(distance);
            transform.localScale = _startScale * distance / _normalDistance;
        }
    }
}