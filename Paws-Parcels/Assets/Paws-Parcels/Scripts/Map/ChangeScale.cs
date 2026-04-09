using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Extentions
{
    public class ChangeScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float _scaleBig = 1.5f;
        [SerializeField] private GameObject _gameObject;
        
        private Vector3 _scale;

        private void Awake()
        {
            _scale = transform.localScale;
            _gameObject?.SetActive(false);
        }
        
        public void OnPointerEnter(PointerEventData eventData = null)
        {
            transform.localScale = _scale * _scaleBig;
            _gameObject?.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData = null)
        {
            transform.localScale = _scale;
            _gameObject?.SetActive(false);
            //GetComponent<Image>().alphaHitTestMinimumThreshold
        }
        
        private void OnDisable()
        {
            OnPointerExit();
        }
    }
}