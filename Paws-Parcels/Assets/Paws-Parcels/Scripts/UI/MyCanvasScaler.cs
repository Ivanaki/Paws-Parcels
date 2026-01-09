using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(CanvasScaler))]
    public class MyCanvasScaler : MonoBehaviour
    {
        private CanvasScaler _canvasScaler;
        
        private void Awake()
        {
            _canvasScaler = GetComponent<CanvasScaler>();
        }

        private void Start()
        {
            var sizeFulHd = 1920f/1080f;
            var size = (float)Screen.width/Screen.height;
            _canvasScaler.matchWidthOrHeight = size >= sizeFulHd ? 1f : 0f;
        }
    }
}