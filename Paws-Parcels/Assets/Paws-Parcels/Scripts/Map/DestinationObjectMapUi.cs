using Quests;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Map
{
    public class DestinationObjectMapUi : MapObject
    {
        [SerializeField] private Image _colorChangeImage1;
        [SerializeField] private Image _colorChangeImage2;
        
        private CompositeDisposable _disposable;
        
        public override void Construct(IShowableOnMap showableOnMap)
        {
            var destinationObject = showableOnMap as DestinationObject;
            
            if (destinationObject == null)
            {
                Debug.LogWarning("InteractableUIObject has no InteractableObject");
                return;
            }
            
            _disposable = new CompositeDisposable();
            
            _disposable.Add(destinationObject.IsQuestSelect.Subscribe(v =>
            {
                UpdateColor(destinationObject);
            }));
            
            _disposable.Add(destinationObject.IsQuestActive.Subscribe(v =>
            {
                UpdateColor(destinationObject);
            }));
        }
        
        private void UpdateColor(DestinationObject destinationObject)
        {
            var activeYellow = destinationObject.IsQuestSelect.CurrentValue &&
                               destinationObject.IsQuestActive.CurrentValue;
            _colorChangeImage1.color = activeYellow ? Color.yellow : Color.white;
            _colorChangeImage2.color = activeYellow ? Color.yellow : Color.white;
        }
        
        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}