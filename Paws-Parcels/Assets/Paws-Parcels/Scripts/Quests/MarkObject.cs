using NaughtyAttributes;
using R3;
using Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Quests
{
    public class MarkObject : MonoBehaviour, IShowableInUIObject
    {
        /*public virtual ReadOnlyReactiveProperty<bool> IsInteract { get; } = new ReactiveProperty<bool>();
        public virtual ReadOnlyReactiveProperty<bool> IsInteractWithMinDistance { get; }  = new ReactiveProperty<bool>();*/
        public ReadOnlyReactiveProperty<bool> IsSelected => _isSelected;
        
        public string Id => _id;
        public string UiTypeId => _uiTypeId;
        public Transform Transform => transform;
        
        protected bool showOnCanvas => _showOnCanvas;
        protected string uiTypeId => _uiTypeId;
        protected CanvasUiVisualizer canvasUiVisualizer => _canvasUiVisualizer;
        
        
        private readonly ReactiveProperty<bool> _isSelected = new();
        //private readonly ReactiveProperty<bool> _isActive = new();
        
        [Header("MarkObject Settings")]
        [SerializeField] private string _id;
        [SerializeField] private bool _showOnCanvas = false;
        [ShowIf(nameof(_showOnCanvas))]
        [SerializeField] private string _uiTypeId;

        [Inject] private DestinationService _destinationService;
        [Inject] private CanvasUiVisualizer _canvasUiVisualizer;
        
        //private CompositeDisposable _disposables = new();

        protected virtual void Awake()
        {
            HideWhenQuestActive();
        }

        protected virtual void OnEnable()
        {
            _destinationService.AddMark(this);
        }

        protected virtual void OnDisable()
        {
            _destinationService.RemoveMark(this);
        }

        public virtual void ShowWhenQuestActive()
        {
            //_isActive.Value = true;
            if(_showOnCanvas)
                _canvasUiVisualizer.ShowOnCanvas(this);
        }

        public virtual void HideWhenQuestActive()
        {
            //_isActive.Value = false;
            if(_showOnCanvas)
                _canvasUiVisualizer.HideOnCanvas(this);
        }

        public virtual void Select()
        {
            _isSelected.Value = true;
        }

        public virtual void Unselect()
        {
            _isSelected.Value = false;
        }
    }
}