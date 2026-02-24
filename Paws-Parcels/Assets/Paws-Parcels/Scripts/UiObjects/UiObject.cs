using System;
using R3;
using UnityEngine;

namespace Services
{
    public abstract class UiObject : MonoBehaviour
    {
        public RectTransform RectTransform => _rectTransform;
        public string UiTypeId => _uiTypeId;
        
        [SerializeField] private string _uiTypeId;
        private RectTransform _rectTransform;
        
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public abstract void Construct(ShowableInUIBehaviour showableInUIObject);
    }
}