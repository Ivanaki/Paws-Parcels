using System;
using UnityEngine;
using UnityEngine.UI;

namespace Paws_Parcels.Scripts
{
    [RequireComponent(typeof(RectTransform))]
    public class PositionHalfOfHeightContentSizeFitter : ContentSizeFitter
    {
        private LayoutElement _parentLayoutElement;
        private RectTransform _rectTransform;
        
        protected override void Awake()
        {
            base.Awake();
            _rectTransform = GetComponent<RectTransform>();
            _parentLayoutElement = GetComponentInParent<LayoutElement>();
        }
        
        public override void SetLayoutVertical()
        {
            base.SetLayoutVertical();
            
            var pos = _rectTransform.anchoredPosition;
            pos.y = - _rectTransform.rect.height / 2f;
            _rectTransform.anchoredPosition = pos;
            if(_parentLayoutElement != null)
                _parentLayoutElement.preferredHeight = _rectTransform.rect.height;
        }
        
/*#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
            if (_parentLayoutElement == null)
            {
                _parentLayoutElement = GetComponentInParent<LayoutElement>();
                print(_parentLayoutElement.name);
            }
        }

#endif*/
    }
}