using System;
using Quests;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Services
{
    public class MarkUIObject : UiObject
    {
        [SerializeField] private Image _image;

        private IDisposable _disposable;
        
        public override void Construct(IShowableInUIObject showableInUIObject)
        {
            var markObject = showableInUIObject as MarkObject;
            
            if (markObject == null)
            {
                Debug.LogWarning("MarkUIObject has no MarkObject");
                return;
            }

            _disposable = markObject.IsSelected.Subscribe(v =>
            {
                _image.color = v ? Color.yellow : Color.white;
            });
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}