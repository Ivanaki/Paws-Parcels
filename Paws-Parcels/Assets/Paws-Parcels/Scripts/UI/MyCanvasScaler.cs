using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(CanvasScaler))]
    public class MyCanvasScaler : MonoBehaviour
    {
        private CanvasScaler _canvasScaler;
        
        private void OnRectTransformDimensionsChange()
        {
            Start();
        }

        private void Start()
        {
            _canvasScaler = GetComponent<CanvasScaler>();
            var sizeFulHd = 1920f/1080f;
            var size = (float)Screen.width/Screen.height;
            _canvasScaler.matchWidthOrHeight = size >= sizeFulHd ? 1f : 0f;
        }
    }
}