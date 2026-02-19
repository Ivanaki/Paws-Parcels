using System;
using UnityEngine;

namespace MotorHome.Scripts
{
    public class CopyCameraTransformAndSetScale : MonoBehaviour
    {
        private const float NormalDistance = 6.5f;

        [SerializeField] private GameObject _canvas;
        [SerializeField, Range(0f,1f)] private float _range = 1f;
        //[SerializeField] private float _time = 0.1f;
        
        
        private Vector3 _startScale;
        //private bool _fix = false;
        private float _startTime;

        private void Awake()
        {
            _startScale = _canvas.transform.localScale;
            _startTime = Time.time;
        }

        private void Update()
        {
            /*if (Time.time - _startTime > _time)
            {
                _startTime = Time.time;
                _canvas.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position, _range);
            }*/
            
            _canvas.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position, _range);
            
            _canvas.transform.rotation = Camera.main.transform.rotation;
            var distance = Vector3.Distance(Camera.main.transform.position, _canvas.transform.position);
            _canvas.transform.localScale = _startScale * distance / NormalDistance;
        }
    }
}