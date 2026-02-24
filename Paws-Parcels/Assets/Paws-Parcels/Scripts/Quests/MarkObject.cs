using NaughtyAttributes;
using R3;
using Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Quests
{
    public class MarkObject : ShowableInUIBehaviour
    {
        /*public virtual ReadOnlyReactiveProperty<bool> IsInteract { get; } = new ReactiveProperty<bool>();
        public virtual ReadOnlyReactiveProperty<bool> IsInteractWithMinDistance { get; }  = new ReactiveProperty<bool>();*/
        public ReadOnlyReactiveProperty<bool> IsSelected => _isSelected;
        
        public string MarkId => _markId;
        public override string UiTypeId => _uiTypeId;
        
        protected bool showOnCanvas => _showOnCanvas;
        protected string uiTypeId => _uiTypeId;
        protected CanvasUiVisualizer canvasUiVisualizer => _canvasUiVisualizer;
        
        private readonly ReactiveProperty<bool> _isSelected = new();
        //private readonly ReactiveProperty<bool> _isActive = new();
        
        [Header("MarkObject Settings")] 
        [SerializeField] private bool _dontChangeMarkId = false;
        [SerializeField] private string _markId;
        [SerializeField] private bool _showOnCanvas = false;
        [ShowIf(nameof(_showOnCanvas))]
        [SerializeField] private string _uiTypeId = "base";

        private bool _isEnabled = false;

        [Inject] private DestinationService _destinationService;
        [Inject] private CanvasUiVisualizer _canvasUiVisualizer;

        protected void SetMarkId(string markId)
        {
            if(_dontChangeMarkId) return;
            
            if (_isEnabled == false)
            {
                _markId = markId;
            }
            else
            {
                OnDisable();
                _markId = markId;
                OnEnable();
            }
        }
        
        protected virtual void OnEnable()
        {
            _isEnabled = true;
            _destinationService.AddMark(this);
        }

        protected virtual void OnDisable()
        {
            _isEnabled = false;
            _destinationService.RemoveMark(this);
        }

        public virtual void ShowWhenQuestActive()
        {
            if(_showOnCanvas)
                _canvasUiVisualizer.ShowOnCanvas(this);
        }

        public virtual void HideWhenQuestActive()
        {
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