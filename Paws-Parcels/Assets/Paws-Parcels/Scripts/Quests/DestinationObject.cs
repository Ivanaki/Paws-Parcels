using System;
using System.Collections.Generic;
using Map;
using NaughtyAttributes;
using R3;
using Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Quests
{
    public abstract class DestinationObject : MonoBehaviour, IShowableInUI, IShowableOnMap
    {
        [Header("Destination settings")] 
        [Tooltip("Can be null")]
        [SerializeField] protected Transform _destinationShowUiPosition;
        
        public abstract string DestinationId { get; }
        public abstract string UiShowTypeId { get; }
        public virtual string MapShowTypeId => "";

        protected virtual float SelectedActivationDistance => 15f;
        protected virtual float ActivationDistance => 7f;
        
        public abstract ReadOnlyReactiveProperty<bool> IsDestinationShowInUi { get; }

        public Observable<IReadOnlyList<string>> QuestsUseThisDestination => _questsUseThisDestination;

        public virtual ReadOnlyReactiveProperty<bool> IsQuestActive => _isQuestActive;
        public virtual ReadOnlyReactiveProperty<bool> IsQuestSelect => _isQuestSelect;
        
        private readonly ReactiveProperty<bool> _isQuestActive = new(false);
        private readonly ReactiveProperty<bool> _isQuestSelect = new(false);
        
        //private readonly ReactiveProperty<bool> _isDestinationShow = new(false);
        
        protected CompositeDisposable _disposables;
 
        [Inject] private DestinationService _destinationService;
        [Inject] protected CanvasUiVisualizer _canvasUiVisualizer;
        [Inject] protected MapUiVisualizer _mapUiVisualizer;

        private BehaviorSubject<IReadOnlyList<string>> _questsUseThisDestination = new(null);
        
        public virtual void QuestActivated(List<string> questId)
        {
            _questsUseThisDestination.OnNext(questId);
            _isQuestActive.Value = true;
        }

        public virtual void QuestDeactived()
        {
            _questsUseThisDestination.OnNext(null);
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
        
        public Quaternion GetGlobalRotationShowablePoint() => 
            _destinationShowUiPosition == null ? transform.rotation : _destinationShowUiPosition.rotation;

        public bool IsDestroyed() => this == null;
        
        public string GetQuestId() => DestinationId;
        
        protected virtual void OnEnable()
        {
            _disposables = new CompositeDisposable();
            _destinationService.AddDestination(this);
            _disposables.Add(IsDestinationShowInUi.Subscribe(show =>
            {
                if (show)
                    _canvasUiVisualizer.ShowOnCanvas(this);
                else
                    _canvasUiVisualizer.HideOnCanvas(this);
            }));
            _disposables.Add(IsQuestActive.Subscribe(show =>
            {
                /*if (show)
                    _mapUiVisualizer.Show(this);
                else
                    _mapUiVisualizer.Hide(this);*/
            }));
        }

        protected virtual void OnDisable()
        {
            _disposables?.Dispose();
            _destinationService.RemoveDestination(this);
        }
    }
}