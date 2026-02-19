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
        public ReadOnlyReactiveProperty<bool> IsSelected => _isSelected;
        public string Id => _id;
        public string UiTypeId => _uiTypeId;
        public Transform Transform => transform;
        
        private readonly ReactiveProperty<bool> _isSelected = new();
        
        [SerializeField] private string _id;
        
        [SerializeField] private GameObject _showableObject;
        [SerializeField] private Behaviour _showableComponent;
        
        [SerializeField] private bool _showOnCanvas = false;
        [ShowIf(nameof(_showOnCanvas))]
        [SerializeField] private string _uiTypeId;

        [Inject] private DestinationService _destinationService;
        [Inject] private CanvasUiVisualizer _canvasUiVisualizer;

        private void Awake()
        {
            Hide();
        }

        private void OnEnable()
        {
            _destinationService.AddMark(this);
        }

        private void OnDisable()
        {
            _destinationService.RemoveMark(this);
        }

        public void Show()
        {
            if(_showOnCanvas)
                _canvasUiVisualizer.ShowOnCanvas(this);
            
            if(_showableObject != null)
                _showableObject?.SetActive(true);
            if(_showableComponent != null)
                _showableComponent.enabled = true;
        }

        public void Hide()
        {
            if(_showOnCanvas)
                _canvasUiVisualizer.HideOnCanvas(this);
            
            if(_showableObject != null)
                _showableObject?.SetActive(false);
            if(_showableComponent != null)
                _showableComponent.enabled = false;
        }

        public void Select()
        {
            _isSelected.Value = true;
            
            if (TryGetComponent(out Image image))
                image.color = Color.yellow;
            
            if(_showableObject != null)
                if (_showableObject.TryGetComponent(out MeshRenderer rendererMesh))
                    rendererMesh.material.color = Color.yellow;
        }

        public void Unselect()
        {
            _isSelected.Value = false;
            
            if (TryGetComponent(out Image image))
                image.color = Color.white;
            
            if(_showableObject != null)
                if (_showableObject.TryGetComponent(out MeshRenderer rendererMesh))
                    rendererMesh.material.color = Color.white;
        }
    }
}