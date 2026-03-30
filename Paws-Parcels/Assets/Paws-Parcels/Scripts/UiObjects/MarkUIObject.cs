using System;
using Quests;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Services
{
    public class MarkUIObject : UiObject
    {
        [SerializeField] private Image _image1;
        [SerializeField] private Image _image2;
        //[SerializeField] private Image _handShowImage;

        private CompositeDisposable _disposable;
        
        public override void Construct(IShowableInUI showableInUIObject)
        {
            var markObject = showableInUIObject as DestinationObject;
            
            if (markObject == null)
            {
                Debug.LogWarning("MarkUIObject has no MarkObject");
                return;
            }

            _disposable = new CompositeDisposable();
            
            _disposable.Add(markObject.IsQuestSelect.Subscribe(v =>
            {
                _image1.color = v ? Color.yellow : Color.white;
                _image2.color = v ? Color.yellow : Color.white;
                //_handShowImage.color = v ? Color.yellow : Color.white;
            }));
            
            /*_disposable.Add(markObject.IsInteract.Subscribe(v =>
            {
                _image.enabled = v;
            }));

            _disposable.Add(markObject.IsInteractWithMinDistance.Subscribe(v =>
            {
                _handShowImage.gameObject.SetActive(v);
            }));*/
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}