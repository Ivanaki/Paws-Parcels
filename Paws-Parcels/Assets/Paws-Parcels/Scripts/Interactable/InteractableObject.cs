using Quests;
using R3;
using UnityEngine;

namespace Paws_Parcels.Scripts.Player
{
    public abstract class InteractableObject : MarkObject
    {
        public ReadOnlyReactiveProperty<bool> IsInteractWithMinDistance => _isInteractWithMinDistance;
        
        private readonly ReactiveProperty<bool> _isInteractWithMinDistance = new();
        
        //private readonly ReactiveProperty<bool> _isActiveQuest = new();
        
        public virtual void Interact()
        {
            print("interact");
        }

        public override void ShowWhenQuestActive()
        {
        }
        
        public override void HideWhenQuestActive()
        {
        }

        public virtual void CanInteractWithMinDistance()
        {
            _isInteractWithMinDistance.Value = true;
        }

        public virtual void CouldntInteractWithMinDistance()
        {
            _isInteractWithMinDistance.Value = false;
        }
        
        public virtual void CanInteract()
        {
            //_isInteract.Value = true;
            
            if(showOnCanvas)
                canvasUiVisualizer.ShowOnCanvas(this);
        }
        
        public virtual void CouldntInteract()
        {
            if(showOnCanvas)
                canvasUiVisualizer.HideOnCanvas(this);
            //_isInteract.Value = false;
        }
    }
}