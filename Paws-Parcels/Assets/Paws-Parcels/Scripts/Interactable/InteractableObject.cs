using Quests;
using R3;
using UnityEngine;

namespace Paws_Parcels.Scripts.Player
{
    //обязательно коллайдер на объекте или на дочернем(со скриптом redirector)
    public abstract class InteractableObject : MarkObject
    {
        //TODO сделать проверку лучом от игрока до объекта
        //[SerializeField] private bool _needCheckWithRay = false;
        
        public ReadOnlyReactiveProperty<bool> IsInteractWithMinDistance => _isInteractWithMinDistance;
        public ReadOnlyReactiveProperty<bool> IsActiveWithQuest => _isActiveWithQuest;
        
        private readonly ReactiveProperty<bool> _isInteractWithMinDistance = new(false);
        private readonly ReactiveProperty<bool> _isActiveWithQuest = new(false);
        
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
            if(showOnCanvas)
                canvasUiVisualizer.ShowOnCanvas(this);
        }
        
        public virtual void CouldntInteract()
        {
            if(showOnCanvas)
                canvasUiVisualizer.HideOnCanvas(this);
        }
    }
}