using System;
using System.Collections.Generic;
using System.Globalization;
using ObservableCollections;
using Paws_Parcels.Scripts;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Quests;
using R3;
using TMPro;
using UI.Core;
using UI.MVVM;
using UI.Screens.LoadMenu;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Quests
{
    public class ScreenQuestsViewModel :  WindowViewModel
    {
        public override string Id => "Quests";
        
        public ReadOnlyReactiveProperty<CultureInfo> CurrentCultureInfo { get; }
        //public IObservableCollection<OneSaveViewModel> OneSaveViewModels => _oneSaveViewModels;
        
        private readonly CoreUIManager _coreUIManager;
        private readonly QuestsServiceForUi _questsServiceForUi;
        
        private readonly ObservableList<OneQuestViewModel> _oneQuestViewModels = new();
        private readonly Dictionary<string, OneSaveViewModel> _questId_OneSaveViewModelDict = new();

        public ScreenQuestsViewModel(CoreUIManager coreUIManager, QuestsServiceForUi questsServiceForUi)
        {
            _coreUIManager = coreUIManager;
            _questsServiceForUi = questsServiceForUi;
        }

        public void RequestBackToGame()
        {
            _coreUIManager.TryCloseScreen();
        }
        
        
        
    }

    public class OneQuestViewModel
    {
        public string QuestId { get; }
        public ReadOnlyReactiveProperty<string> TextToShow => _textToShow;
        
        private readonly ReactiveProperty<string> _textToShow = new("test");
        
        public OneQuestViewModel(string questId)
        {
            QuestId = questId;
        }
    }

    public class OneQuestBinder : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Image _image;
        
        [SerializeField] private MouseEnterUtil _mouseEnterUtil;
        
        private readonly CompositeDisposable _disposables = new();
        
        private Color _selectedColor;
        private Color _nonSelectedColor;
        private Color _mouseEnterColor;

        private OneSaveViewModel _viewModel;

        public void Initialize(Color selectedColor, Color nonSelectedColor, Color mouseEnterColor)
        {
            _selectedColor = selectedColor;
            _nonSelectedColor = nonSelectedColor;
            _mouseEnterColor = mouseEnterColor;
        }
        
        public void Bind(OneSaveViewModel viewModel)
        {
            _viewModel = viewModel;
            
            _disposables.Add(_button.OnClickAsObservable().Subscribe(_ => viewModel.RequestClicked()));
            _disposables.Add(viewModel.TextToShow.Subscribe(v => _nameText.text = v));
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