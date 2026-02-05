using System.Collections.Generic;
using System.Linq;
using ObservableCollections;
using R3;
using TMPro;
using UI.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.LoadMenu
{
    public class PopupLoadMenuBinder : PopupBinder<PopupLoadMenuViewModel>
    {
        [SerializeField] private Button _loadButton;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private Button _updateButton;

        [SerializeField] private TMP_Text _currentMissionText;
        [SerializeField] private TMP_Text _currentLocationText;
        [SerializeField] private TMP_Text _saveFolderNameText;
        //[SerializeField] private TMP_Text _dateTimeText;
        [SerializeField] private TMP_Text _saveVersionText;
        [SerializeField] private TMP_Text _gameVersionText;

        [SerializeField] private RawImage _screenshotRawImage;
        
        [SerializeField] private Transform _prefabsGrid;
        [SerializeField] private OneSaveBinder _oneSaveBinderPrefab;
        
        [SerializeField] private Color _nonSelectedColor;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _mouseEnterColor;
        
        private readonly Dictionary<OneSaveViewModel, OneSaveBinder> _instantiatedOneSaveBindersMap = new();
        private readonly CompositeDisposable _disposables = new();
        
        protected override void OnBind(PopupLoadMenuViewModel viewModel)
        {
            //поддерживаем биндеры
            //от меньшей даты к большей
            foreach (var oneSaveViewModel in viewModel.OneSaveViewModels)
            {
                InsertOneSaveBinder(oneSaveViewModel, 0);
            }
            
            _disposables.Add(viewModel.OneSaveViewModels.ObserveAdd().Subscribe(e =>
            {
                var placeIndex = viewModel.OneSaveViewModels.Count - e.Index -1;
                InsertOneSaveBinder(e.Value, placeIndex);
            }));
            
            _disposables.Add(viewModel.OneSaveViewModels.ObserveRemove().Subscribe(e =>
            {
                DestroyOneSaveBinder(e.Value);
            }));
            
            _disposables.Add(viewModel.OneSaveViewModels.ObserveClear().Subscribe(e =>
            {
                DestroyAllSaveBinders();
            }));
            
            //кнопочки
            _disposables.Add(_loadButton.OnClickAsObservable().Subscribe(_ => viewModel.RequestLoadGame()));
            _disposables.Add(_deleteButton.OnClickAsObservable().Subscribe(_ => viewModel.RequestDeleteSave()));
            _disposables.Add(_updateButton.OnClickAsObservable().Subscribe(_ => viewModel.RequestUpdateDataInFolderOfSave()));
            
            //поля
            _disposables.Add(viewModel.ScreenshotTexture2D.Subscribe(v => _screenshotRawImage.texture = v));
            
            _disposables.Add(viewModel.CurrentMissionText.Subscribe(v => _currentMissionText.text = v));
            _disposables.Add(viewModel.CurrentLocationText.Subscribe(v => _currentLocationText.text = v));
            _disposables.Add(viewModel.SaveNameText.Subscribe(v => _saveFolderNameText.text = v));
            //_disposables.Add(viewModel.DateTimeText.Subscribe(v => _dateTimeText.text = v));
            _disposables.Add(viewModel.SaveVersionText.Subscribe(v => _saveVersionText.text = v));
            _disposables.Add(viewModel.GameVersionText.Subscribe(v => _gameVersionText.text = v));
        }
        
        //методы поддержки
        private void InsertOneSaveBinder(OneSaveViewModel oneSaveViewModel, int index)
        {
            var oneSaveBinder = Instantiate(_oneSaveBinderPrefab, _prefabsGrid);
            oneSaveBinder.transform.SetSiblingIndex(index);
            
            oneSaveBinder.Initialize(_selectedColor, _nonSelectedColor, _mouseEnterColor);
            oneSaveBinder.Bind(oneSaveViewModel);
            
            _instantiatedOneSaveBindersMap[oneSaveViewModel] = oneSaveBinder;
        }
        
        private void DestroyOneSaveBinder(OneSaveViewModel oneSaveViewModel)
        {
            if (_instantiatedOneSaveBindersMap.TryGetValue(oneSaveViewModel, out var oneSaveBinder))
            {
                Destroy(oneSaveBinder.gameObject);
                _instantiatedOneSaveBindersMap.Remove(oneSaveViewModel);
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