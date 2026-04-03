using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Extentions
{
    public class ChangeScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float _scaleBig = 1.5f;
        
        private Vector3 _scale;

        private void Awake()
        {
            _scale = transform.localScale;
        }
        
        public void OnPointerEnter(PointerEventData eventData = null)
        {
            transform.localScale = _scale * _scaleBig;
        }

        public void OnPointerExit(PointerEventData eventData = null)
        {
            transform.localScale = _scale;
            //GetComponent<Image>().alphaHitTestMinimumThreshold
        }
        
        private void OnDisable()
        {
            OnPointerExit();
        }
    }
}