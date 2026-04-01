using System;
using System.Collections.Generic;
using ObservableCollections;
using R3;
using TMPro;
using UI.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Quests
{
    public class ScreenQuestsBinder : WindowBinder<ScreenQuestsViewModel>
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _selectInDataButton;
        
        [SerializeField] private Transform _prefabsGrid;
        [SerializeField] private OneQuestBinder _oneQuestBinderPrefab;
        
        [SerializeField] private Color _nonSelectedColor;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _mouseEnterColor;

        [SerializeField] private TMP_Text _questTitleText;
        [SerializeField] private TMP_Text _missionTitleText;
        [SerializeField] private TMP_Text _missionDescriptionText;
        [SerializeField] private TMP_Text _missionLimitTextText;
        [SerializeField] private TMP_Text _questRewardMoneyText;
        
        private readonly Dictionary<OneQuestViewModel, OneQuestBinder> _instantiatedOneQuestBindersMap = new();
        
        private CompositeDisposable _disposables;

        protected override void OnBind(ScreenQuestsViewModel viewModel)
        {
            for (int i = _prefabsGrid.childCount - 1; i >= 0; i--)
            {
                Destroy(_prefabsGrid.GetChild(i).gameObject);
            }
            
            _disposables = new();
            
            //поддерживаем биндеры
            //от меньшей даты к большей
            foreach (var oneQuestViewModel in viewModel.OneQuestViewModels)
            {
                AddOneQuestBinder(oneQuestViewModel);
            }
            
            _disposables.Add(viewModel.OneQuestViewModels.ObserveAdd().Subscribe(e =>
            {
                var placeIndex = viewModel.OneQuestViewModels.Count - e.Index -1;
                InsertOneSaveBinder(e.Value, placeIndex);
            }));
            
            _disposables.Add(viewModel.OneQuestViewModels.ObserveRemove().Subscribe(e =>
            {
                DestroyOneSaveBinder(e.Value);
            }));
            
            _disposables.Add(viewModel.OneQuestViewModels.ObserveClear().Subscribe(e =>
            {
                DestroyAllSaveBinders();
            }));
            
            //кнопочки
            _disposables.Add(_exitButton.OnClickAsObservable().Subscribe(_ => viewModel.RequestBackToGame()));
            _disposables.Add(_selectInDataButton.OnClickAsObservable().Subscribe(_ => viewModel.RequestSelectInData()));
            
            //текст
            _disposables.Add(viewModel.CurrentQuestTitleText.Subscribe(text => _questTitleText.text = text));
            _disposables.Add(viewModel.CurrentMissionTitleText.Subscribe(text => _missionTitleText.text = text));
            _disposables.Add(viewModel.CurrentMissionDescriptionText.Subscribe(text => _missionDescriptionText.text = text));
            _disposables.Add(viewModel.CurrentMissionLimitText.Subscribe(text => _missionLimitTextText.text = text));
            _disposables.Add(viewModel.CurrentQuestRewardMoneyText.Subscribe(text => _questRewardMoneyText.text = text));
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

        private void AddOneQuestBinder(OneQuestViewModel oneQuestViewModel)
        {
            var oneQuestBinder = Instantiate(_oneQuestBinderPrefab, _prefabsGrid);
            
            oneQuestBinder.Initialize(_selectedColor, _nonSelectedColor, _mouseEnterColor);
            oneQuestBinder.Bind(oneQuestViewModel);
            
            _instantiatedOneQuestBindersMap[oneQuestViewModel] = oneQuestBinder;
        }
        
        private void InsertOneSaveBinder(OneQuestViewModel oneQuestViewModel, int index)
        {
            var oneQuestBinder = Instantiate(_oneQuestBinderPrefab, _prefabsGrid);
            oneQuestBinder.transform.SetSiblingIndex(index);
            
            oneQuestBinder.Initialize(_selectedColor, _nonSelectedColor, _mouseEnterColor);
            oneQuestBinder.Bind(oneQuestViewModel);
            
            _instantiatedOneQuestBindersMap[oneQuestViewModel] = oneQuestBinder;
        }
        
        private void DestroyOneSaveBinder(OneQuestViewModel oneQuestViewModel)
        {
            if (_instantiatedOneQuestBindersMap.TryGetValue(oneQuestViewModel, out var oneSaveBinder))
            {
                Destroy(oneSaveBinder.gameObject);
                _instantiatedOneQuestBindersMap.Remove(oneQuestViewModel);
            }
        }

        private void DestroyAllSaveBinders()
        {
            foreach (Transform child in _prefabsGrid)
            {
                Destroy(child.gameObject);
            }
        }
    }
}