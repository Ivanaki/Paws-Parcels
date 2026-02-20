using Quests;
using R3;
using UnityEngine;

namespace Paws_Parcels.Scripts.Player
{
    public abstract class InteractableObject : MarkObject
    {
        public ReadOnlyReactiveProperty<bool> IsInteractWithMinDistance => _isInteractWithMinDistance;
        public ReadOnlyReactiveProperty<bool> IsActiveWithQuest => _isActiveWithQuest;
        
        private readonly ReactiveProperty<bool> _isInteractWithMinDistance = new();
        private readonly ReactiveProperty<bool> _isActiveWithQuest = new();
        
        //private readonly ReactiveProperty<bool> _isActiveQuest = new();
        
        public virtual void Interact()
        {
            print("interact not overrided");
        }

        public override void ShowWhenQuestActive()
        {
            _isActiveWithQuest.Value = true;
        }
        
        public override void HideWhenQuestActive()
        {
            _isActiveWithQuest.Value = false;
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