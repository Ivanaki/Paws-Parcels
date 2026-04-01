using Paws_Parcels.Scripts;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Quests
{
    public class OneQuestBinder : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Image _image;
        [SerializeField] private GameObject _selectedInDataGameObject;
        
        [SerializeField] private MouseEnterUtil _mouseEnterUtil;
        
        private readonly CompositeDisposable _disposables = new();
        
        private Color _selectedColor;
        private Color _nonSelectedColor;
        private Color _mouseEnterColor;

        private OneQuestViewModel _viewModel;

        public void Initialize(Color selectedColor, Color nonSelectedColor, Color mouseEnterColor)
        {
            _selectedColor = selectedColor;
            _nonSelectedColor = nonSelectedColor;
            _mouseEnterColor = mouseEnterColor;
        }
        
        public void Bind(OneQuestViewModel viewModel)
        {
            _viewModel = viewModel;
            
            _disposables.Add(viewModel.IsSelectedIdData.Subscribe(v =>
            {
                _selectedInDataGameObject.SetActive(v);
            }));
            _disposables.Add(_button.OnClickAsObservable().Subscribe(_ => viewModel.RequestClicked()));
            _disposables.Add(viewModel.TextToShow.Subscribe(v =>
            {
                _nameText.text = viewModel.IsCompleted ? "<s>" + v : v;
            }));
            _disposables.Add(viewModel.IsSelected.Subscribe(v =>
            {
                if (v) SetImageColor(_selectedColor);
                else SetImageColor(_mouseEnterUtil.IsMouseEnter.CurrentValue ? _mouseEnterColor : _nonSelectedColor);;
            }));
            _disposables.Add(_mouseEnterUtil.IsMouseEnter.Subscribe(v =>
            {
                if (_viewModel != null && _viewModel.IsSelected.CurrentValue == false)
                {
                    SetImageColor(v ? _mouseEnterColor : _nonSelectedColor);
                }
            }));
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void SetImageColor(Color color)
        {
            _image.color = color;
        }
    }
}