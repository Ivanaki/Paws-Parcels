using System;
using NaughtyAttributes;
using R3;
using Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Quests
{
    public abstract class DestinationObject : MonoBehaviour, IShowableInUI
    {
        [Header("Destination settings")] 
        [Tooltip("Can be null")]
        [SerializeField] private Transform _destinationShowUiPosition;
        
        public abstract string DestinationId { get; }
        public abstract string UiShowTypeId { get; }
        public abstract string UiShowText { get; }
        
        
        public virtual ReadOnlyReactiveProperty<bool> IsQuestActive => _isQuestActive;
        public virtual ReadOnlyReactiveProperty<bool> IsQuestSelect => _isQuestSelect;
        
        private readonly ReactiveProperty<bool> _isQuestActive = new(false);
        private readonly ReactiveProperty<bool> _isQuestSelect = new(false);
 
        [Inject] private DestinationService _destinationService;
        
        public virtual void QuestActivated()
        {
            _isQuestActive.Value = true;
        }

        public virtual void QuestDeactived()
        {
            _isQuestActive.Value = false;
        }

        public virtual void QuestSelected()
        {
            _isQuestSelect.Value = true;
        }

        public virtual void QuestUnselected()
        {
            _isQuestSelect.Value = false;
        }

        public Vector3 GetGlobalPositionShowablePoint() =>
            _destinationShowUiPosition == null ? transform.position : _destinationShowUiPosition.position;
        

        //TODO проверить на работоспособность других вариантов проверки на null
        public bool IsDestroyed() => this == null;
        
        protected virtual void OnEnable()
        {
            _destinationService.AddDestination(this);
        }

        protected virtual void OnDisable()
        {
            _destinationService.RemoveDestination(this);
        }
    }
}