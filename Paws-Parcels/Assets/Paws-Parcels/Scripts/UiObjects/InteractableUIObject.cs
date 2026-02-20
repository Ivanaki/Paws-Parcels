using Paws_Parcels.Scripts.Player;
using Quests;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Services
{
    public class InteractableUIObject : UiObject
    {
        [SerializeField] private Image _image;
        [SerializeField] private Image _handShowImage;

        private CompositeDisposable _disposable;
        
        public override void Construct(IShowableInUIObject showableInUIObject)
        {
            var interactableObject = showableInUIObject as InteractableObject;
            
            if (interactableObject == null)
            {
                Debug.LogWarning("InteractableUIObject has no InteractableObject");
                return;
            }

            _disposable = new CompositeDisposable();
            
            _disposable.Add(interactableObject.IsSelected.Subscribe(v =>
            {
                _image.color = v ? Color.yellow : Color.white;
                _handShowImage.color = v ? Color.yellow : Color.white;
            }));
            
            /*_disposable.Add(interactableObject.IsInteract.Subscribe(v =>
            {
                _image.enabled = v;
            }));*/

            _disposable.Add(interactableObject.IsInteractWithMinDistance.Subscribe(v =>
            {
                _handShowImage.gameObject.SetActive(v);
            }));
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}