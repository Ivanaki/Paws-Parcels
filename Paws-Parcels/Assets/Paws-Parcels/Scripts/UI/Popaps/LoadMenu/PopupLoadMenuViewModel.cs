using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ObservableCollections;
using Paws_Parcels.Scripts;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using UI.MVVM;
using UnityEngine;

namespace UI.Screens.LoadMenu
{
    public class PopupLoadMenuViewModel : WindowViewModel
    {
        private const string NoneSimbol = "-";
        private static readonly Texture2D NoneTexture = Utils.CreateWhiteTexture();
        
        public override string Id => "LoadMenu";
        
        public IObservableCollection<OneSaveViewModel> OneSaveViewModels => _oneSaveViewModels;

        public ReadOnlyReactiveProperty<string> CurrentMissionText => _currentMissionText;
        public ReadOnlyReactiveProperty<string> CurrentLocationText => _currentLocationText;
        public ReadOnlyReactiveProperty<string> SaveNameText => _saveNameText;
        public ReadOnlyReactiveProperty<string> DateTimeText => _dateTimeText;
        public ReadOnlyReactiveProperty<string> SaveVersionText => _saveVersionText;
        public ReadOnlyReactiveProperty<string> GameVersionText => _gameVersionText;
        public ReadOnlyReactiveProperty<Texture2D> ScreenshotTexture2D => _screenshotTexture2D;
        
        public ReadOnlyReactiveProperty<CultureInfo> CurrentCultureInfo { get; }
        
        private readonly LoadService _loadService;
        private readonly IReadOnlyObservableDictionary<Guid, GameMeta> _metasMap;

        //
        private readonly ObservableList<OneSaveViewModel> _oneSaveViewModels = new();
        private readonly Dictionary<Guid, OneSaveViewModel> _savesMap = new();
        //private readonly Dictionary<OneSaveViewModel, DateTime> _oneSaveViewModelDate = new();
        
        private readonly CompositeDisposable _disposables = new();
        
        private readonly ReactiveProperty<string> _currentMissionText = new(NoneSimbol);
        private readonly ReactiveProperty<string> _currentLocationText = new(NoneSimbol);
        private readonly ReactiveProperty<string> _saveNameText = new(NoneSimbol);
        private readonly ReactiveProperty<string> _dateTimeText = new(NoneSimbol);
        private readonly ReactiveProperty<string> _saveVersionText = new(NoneSimbol);
        private readonly ReactiveProperty<string> _gameVersionText = new(NoneSimbol);
        private readonly ReactiveProperty<Texture2D> _screenshotTexture2D = new(NoneTexture);
        
        
        private readonly ReactiveProperty<DateTime> _dateTimeUtc = new();
        private readonly ReactiveProperty<string> _currentQuestCode = new();
        private readonly ReactiveProperty<string> _currentLocationCode = new();


        private OneSaveViewModel _selectedSaveViewModel = null;
        private Guid _selectedSaveId = Guid.Empty;
        
        public PopupLoadMenuViewModel(LoadService loadService)
        {
            _loadService = loadService;
            CurrentCultureInfo = loadService.CurrentCultureInfo;
            
            _disposables.Add(_dateTimeUtc.Skip(1).Subscribe(dateTime =>
            {
                _dateTimeText.Value = dateTime.ToLocalTime().ToString("f", CurrentCultureInfo.CurrentValue);
            }));

            _disposables.Add(_currentQuestCode.Skip(1).Subscribe(questCode =>
            {
                _currentMissionText.Value = _loadService.GetQuestWithLocalization(questCode);
            }));

            _disposables.Add(_currentLocationCode.Skip(1).Subscribe(locCode =>
            {
                _currentLocationText.Value = _loadService.GetLocationWithLocalization(locCode);
            }));

            _disposables.Add(CurrentCultureInfo.Skip(1).Subscribe(culture =>
            {
                _dateTimeText.Value = _dateTimeUtc.CurrentValue.ToLocalTime().ToString("f", culture);
                _currentMissionText.Value = _loadService.GetQuestWithLocalization(_currentQuestCode.CurrentValue);
                _currentLocationText.Value = _loadService.GetLocationWithLocalization(_currentLocationCode.CurrentValue);
            }));
            
            _metasMap = loadService.MetasMap;
            
            var values = loadService.MetasMap.Values;
            // отсортировано от меньшего к большему. более новое время в конец
            // это сделано для оптимизации, тк обычно мы добавляем сейв с временем больше всех
            var sortedValues = values.OrderBy(x => x.SaveDataTimeUtc).ToList();
            foreach (var gameMeta in sortedValues)
            {
                AddOneSaveViewModel(gameMeta);
            }

            _disposables.Add(loadService.MetasMap.ObserveAdd().Subscribe(e =>
            {
                var gameMeta = e.Value.Value;

                //вставляем элемент в уже отсортированный список большее в конец
                var inserted = false;
                for (var i = _oneSaveViewModels.Count-1; i >= 0; i--)
                {
                    if (gameMeta.SaveDataTimeUtc >= _oneSaveViewModels[i].DateTimeUtc)
                    {
                        InsertOneSaveViewModel(gameMeta, i + 1);
                        inserted = true;
                        break;
                    }
                }
                if (inserted == false) InsertOneSaveViewModel(gameMeta, 0);
            }));

            _disposables.Add(loadService.MetasMap.ObserveRemove().Subscribe(e =>
            {
                var gameMeta = e.Value.Value;
                RemoveOneSaveViewModel(gameMeta);
            }));

            _disposables.Add(loadService.MetasMap.ObserveClear().Subscribe(_ => ClearAllOneSaveViewModels()));
            
            SelectLastSaveViewModel();
        }

        private void InsertOneSaveViewModel(GameMeta gameMeta, int index)
        {
            var viewModel = new OneSaveViewModel(gameMeta, this);
            
            _oneSaveViewModels.Insert(index, viewModel);
            _savesMap[gameMeta.SaveId] = viewModel;
            //_oneSaveViewModelDate[viewModel] = gameMeta.SaveDataTimeUtc;
        }
        
        private void AddOneSaveViewModel(GameMeta gameMeta)
        {
            var viewModel = new OneSaveViewModel(gameMeta, this);
            
            _oneSaveViewModels.Add(viewModel);
            _savesMap[gameMeta.SaveId] = viewModel;
            //_oneSaveViewModelDate[viewModel] = gameMeta.SaveDataTimeUtc;
        }
        
        private void RemoveOneSaveViewModel(GameMeta gameMeta)
        {
            if (_savesMap.TryGetValue(gameMeta.SaveId, out var viewModel))
            {
                _oneSaveViewModels.Remove(viewModel);
                _savesMap.Remove(gameMeta.SaveId);
                //_oneSaveViewModelDate.Remove(viewModel);
            }
        }
        
        private void ClearAllOneSaveViewModels()
        {
            _oneSaveViewModels.Clear();
            _savesMap.Clear();
            //_oneSaveViewModelDate.Clear();
        }

        private void ShowSaveUI(GameMeta gameMeta, Texture2D texture)
        {
            if (gameMeta == null)
            {
                _currentMissionText.Value = NoneSimbol;
                _saveNameText.Value = NoneSimbol;
                _dateTimeText.Value = NoneSimbol;
                _saveVersionText.Value = NoneSimbol;
                _gameVersionText.Value = NoneSimbol;
            }
            else
            {
                //var culture = 
                //var culture = new CultureInfo("en-US");
                //var dateTimeText = gameMeta.SaveDataTimeUtc.ToLocalTime().ToString("f", culture);

                _currentQuestCode.Value = gameMeta.TrackedQuestCode;
                _currentLocationCode.Value = gameMeta.CurrentLocationCode;
                _dateTimeUtc.Value = gameMeta.SaveDataTimeUtc;
                
                _saveNameText.Value = gameMeta.SaveName;
                _saveVersionText.Value = gameMeta.SaveVersion.ToString();
                _gameVersionText.Value = gameMeta.GameVersion.ToString();
            }

            if (texture == null)
            {
                texture = NoneTexture;
            }
            _screenshotTexture2D.Value = texture;
        }

        private void SelectOneSaveViewModel(Guid idSave)
        {
            _selectedSaveId = idSave;
            
            var viewModel = _savesMap.GetValueOrDefault(idSave);
            
            _selectedSaveViewModel?.Unselect();
            _selectedSaveViewModel = viewModel;
            _selectedSaveViewModel?.Select();
        }

        private void SelectLastSaveViewModel()
        {
            RequestOneSaveClicked(_oneSaveViewModels.Count > 0 ? _oneSaveViewModels[^1].Id : Guid.Empty);
        }

        public override void Dispose()
        {
            _disposables.Dispose();
        }
        
        //Requests by binders and child viewModels
        #region Requests by binders and child viewModels
        
        
        public void RequestLoadGame()
        {
            if (_selectedSaveId != Guid.Empty)
            {
                _loadService.LoadGame(_selectedSaveId);
            }
        }

        public void RequestDeleteSave()
        {
            if (_selectedSaveId != Guid.Empty)
            {
                _loadService.DeleteSave(_selectedSaveId);
                SelectLastSaveViewModel();
            }
        }

        public void RequestUpdateDataInFolderOfSave()
        {
            _loadService.UpdateDataInFolderOfSave();
            SelectLastSaveViewModel();
        }

        public void RequestOneSaveClicked(Guid saveId)
        {
            SelectOneSaveViewModel(saveId);

            if (saveId == Guid.Empty)
            {
                ShowSaveUI(null, null);
                return;
            }
            
            var gameMeta = _metasMap.GetValueOrDefault(saveId);
            var texture = _loadService.GetTextureOrDefault(saveId);
            ShowSaveUI(gameMeta, texture);
        }
        
        
        
        #endregion
    }
}