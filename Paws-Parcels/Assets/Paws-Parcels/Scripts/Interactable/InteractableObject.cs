using Quests;
using R3;
using Services;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Scripts.Player
{
    //обязательно коллайдер на объекте или на дочернем(со скриптом redirector)
    public abstract class InteractableObject : DestinationObject
    {
        //TODO сделать проверку лучом от игрока до объекта
        //[SerializeField] private bool _needCheckWithRay = false;

        
        
        
        public ReadOnlyReactiveProperty<bool> CanInteract => _canInteract;
        private readonly ReactiveProperty<bool> _canInteract = new(false);

        [Inject] private CanvasUiVisualizer _canvasUiVisualizer;

        public abstract void Interact();

        public virtual void CanInteractWithMinDistance()
        {
            _canInteract.Value = true;
        }

        public virtual void CouldntInteractWithMinDistance()
        {
            _canInteract.Value = false;
        }
        
        public virtual void Highlight()
        {
            if(string.IsNullOrEmpty(UiShowTypeId) == false)
                _canvasUiVisualizer.ShowOnCanvas(this);
        }
        
        public virtual void Unhighlight()
        {
            if(string.IsNullOrEmpty(UiShowTypeId) == false)
                _canvasUiVisualizer.HideOnCanvas(this);
        }
    }
}