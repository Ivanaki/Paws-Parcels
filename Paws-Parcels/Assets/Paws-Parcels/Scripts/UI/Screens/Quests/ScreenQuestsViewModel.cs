using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ObservableCollections;
using Paws_Parcels.Scripts.Quests;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Quests;
using R3;
using Services;
using UI.Core;
using UI.MVVM;
using UI.Screens.LoadMenu;
using UnityEngine.Localization;

namespace UI.Screens.Quests
{
    public class ScreenQuestsViewModel :  WindowViewModel
    {
        private readonly CoreUIManager _coreUIManager;
        private readonly GameProxy _gameProxy;
        public LocalizationService LocalizationService { get; }

        private const string NoneSimbol = "-";
        
        public override string Id => "Quests";
        
        public IObservableCollection<OneQuestViewModel> OneQuestViewModels => _oneQuestViewModels;

        public ReadOnlyReactiveProperty<string> CurrentQuestTitleText => _currentQuestTitleText;
        public ReadOnlyReactiveProperty<string> CurrentMissionTitleText => _currentMissionTitleText;
        public ReadOnlyReactiveProperty<string> CurrentMissionDescriptionText => _currentMissionDescriptionText;
        public ReadOnlyReactiveProperty<string> CurrentMissionLimitText => _currentMissionLimitText;
        public ReadOnlyReactiveProperty<string> CurrentQuestRewardMoneyText => _currentQuestRewardMoneyText;
        
        
        private IReadOnlyDictionary<string, Quest> QuestsDict { get; }
        private IReadOnlyObservableList<QuestDataProxy> ActiveQuests { get; }
        private IReadOnlyObservableList<string> SucceedQuests { get; }
        public ReadOnlyReactiveProperty<string> SelectedQuestInData { get; }
        

        //
        private readonly ObservableList<OneQuestViewModel> _oneQuestViewModels = new();
        private readonly Dictionary<string, OneQuestViewModel> _questsMap = new();
        
        private readonly CompositeDisposable _disposables = new();
        
        private readonly ReactiveProperty<string> _currentQuestTitleText = new(NoneSimbol);
        private readonly ReactiveProperty<string> _currentMissionTitleText = new(NoneSimbol);
        private readonly ReactiveProperty<string> _currentMissionDescriptionText = new(NoneSimbol);
        private readonly ReactiveProperty<string> _currentMissionLimitText = new(NoneSimbol);
        
        private readonly ReactiveProperty<string> _currentQuestRewardMoneyText = new(NoneSimbol);
        
        private readonly BehaviorSubject<string> _currentQuestTitleCode = new("");
        private readonly BehaviorSubject<string> _currentMissionTitleCode = new("");
        private readonly BehaviorSubject<string> _currentMissionDescriptionCode = new("");
        private readonly BehaviorSubject<Limits> _currentMissionLimitCode = new(Limits.LimitNone);


        private OneQuestViewModel _selectedQuestViewModel = null;
        private string _selectedQuestId = "";
        
        public ScreenQuestsViewModel(CoreUIManager coreUIManager ,LocalizationService localizationService, QuestsDatabase questsDatabase,
            GameProxy gameProxy)
        {
            _coreUIManager = coreUIManager;
            _gameProxy = gameProxy;
            LocalizationService = localizationService;
            QuestsDict = questsDatabase.QuestsDict;
            ActiveQuests = gameProxy.ActiveQuests;
            SucceedQuests = gameProxy.SucceedQuests;
            SelectedQuestInData = gameProxy.SelectedQuest;
            
            _disposables.Add(_currentQuestTitleCode.Skip(1).Subscribe(questCode =>
            {
                _currentQuestTitleText.Value =
                    localizationService.GetCurrentLanguageStringOrDefault(questCode, Tables.Quests);
            }));

            _disposables.Add(_currentMissionTitleCode.Skip(1).Subscribe(questCode =>
            {
                _currentMissionTitleText.Value =
                    localizationService.GetCurrentLanguageStringOrDefault(questCode, Tables.Quests);
            }));

            _disposables.Add(_currentMissionDescriptionCode.Skip(1).Subscribe(questCode =>
            {
                _currentMissionDescriptionText.Value =
                    localizationService.GetCurrentLanguageStringOrDefault(questCode, Tables.Quests);
            }));

            _disposables.Add(_currentMissionLimitCode.Skip(1).Subscribe(questCode =>
            {
                _currentMissionLimitText.Value =
                    localizationService.GetCurrentLanguageStringOrDefault(questCode.ToString(), Tables.Quests);
            }));

            _disposables.Add(localizationService.CurrentLocale.Skip(1).Subscribe(culture =>
            {
                _currentQuestTitleText.Value =
                    localizationService.GetCurrentLanguageStringOrDefault(_currentQuestTitleCode.Value, Tables.Quests);
                _currentMissionTitleText.Value =
                    localizationService.GetCurrentLanguageStringOrDefault(_currentMissionTitleCode.Value, Tables.Quests);
                _currentMissionDescriptionText.Value =
                    localizationService.GetCurrentLanguageStringOrDefault(_currentMissionDescriptionCode.Value, Tables.Quests);
                _currentMissionLimitText.Value =
                    localizationService.GetCurrentLanguageStringOrDefault(_currentMissionLimitCode.Value.ToString(), Tables.Quests);
            }));
            
            
            foreach (var gameMeta in ActiveQuests)
            {
                AddOneQuestViewModel(gameMeta.Id, false);
            }

            foreach (var gameMeta in SucceedQuests)
            {
                AddOneQuestViewModel(gameMeta, true);
            }

            _disposables.Add(ActiveQuests.ObserveAdd().Subscribe(e =>
            {
                var questDataProxy = e.Value;

                //вставляем элемент в начало
                InsertOneSaveViewModel(questDataProxy.Id, false, 0);
            }));

            _disposables.Add(SucceedQuests.ObserveAdd().Subscribe(e =>
            {
                var questId = e.Value;

                //вставляем элемент после выполненных
                var inserted = false;
                for (var i = 0; i < _oneQuestViewModels.Count; i++)
                {
                    var  viewModel = _oneQuestViewModels[i];

                    if (viewModel.IsCompleted)
                    {
                        InsertOneSaveViewModel(questId, true, i);
                        inserted = true;
                        break;
                    }
                }
                if(inserted == false) AddOneQuestViewModel(questId, true);
            }));

            _disposables.Add(ActiveQuests.ObserveRemove().Subscribe(e =>
            {
                var questDataProxy = e.Value;
                RemoveOneQuestViewModel(questDataProxy.Id);
            }));

            _disposables.Add(SucceedQuests.ObserveRemove().Subscribe(e =>
            {
                var questId = e.Value;
                RemoveOneQuestViewModel(questId);
            }));

            _disposables.Add(ActiveQuests.ObserveClear().Subscribe(_ => ClearActiveOneQuestViewModels()));
            _disposables.Add(SucceedQuests.ObserveClear().Subscribe(_ => ClearSucsessOneQuestViewModels()));
            
            SelectLastSaveViewModel();
        }
        
        private void InsertOneSaveViewModel(string questId, bool isComplete, int index)
        {
            var viewModel = new OneQuestViewModel(this, questId, isComplete);
            
            _oneQuestViewModels.Insert(index, viewModel);
            _questsMap[questId] = viewModel;
        }
        
        private void AddOneQuestViewModel(string questId, bool isComplete)
        {
            var viewModel = new OneQuestViewModel(this, questId, isComplete);
            
            _oneQuestViewModels.Add(viewModel);
            _questsMap[questId] = viewModel;
        }
        
        private void RemoveOneQuestViewModel(string questId)
        {
            if (_questsMap.TryGetValue(questId, out var viewModel))
            {
                _oneQuestViewModels.Remove(viewModel);
                _questsMap.Remove(questId);
            }
        }
        
        private void ClearActiveOneQuestViewModels()
        {
            for (var i = _oneQuestViewModels.Count - 1; i >= 0; i--)
            {
                var viewModel = _oneQuestViewModels[i];

                if (viewModel.IsCompleted == false)
                {
                    _questsMap.Remove(viewModel.QuestId);
                    _oneQuestViewModels.RemoveAt(i);
                }
            }
        }

        private void ClearSucsessOneQuestViewModels()
        {
            for (var i = _oneQuestViewModels.Count - 1; i >= 0; i--)
            {
                var viewModel = _oneQuestViewModels[i];

                if (viewModel.IsCompleted)
                {
                    _questsMap.Remove(viewModel.QuestId);
                    _oneQuestViewModels.RemoveAt(i);
                }
            }
        }

        private void ShowSaveUINull()
        {
            _currentQuestTitleText.Value = NoneSimbol;
            _currentMissionTitleText.Value = NoneSimbol;
            _currentMissionDescriptionText.Value = NoneSimbol;
            _currentMissionLimitText.Value = NoneSimbol;

            _currentQuestRewardMoneyText.Value = NoneSimbol;
        }

        private void ShowSaveUI(Quest quest)
        {
            if (_questsMap[quest.Id].IsCompleted)
            {
                _currentQuestRewardMoneyText.Value = quest.MoneyReward.ToString();
                _currentQuestTitleCode.OnNext(quest.Id);
                _currentMissionLimitText.Value = NoneSimbol;
                
                //_currentMissionTitleText.Value = LocalizationService.GetCurrentLanguageStringOrDefault("");
                _currentMissionDescriptionText.Value = NoneSimbol;
            }
            else
            {
                var currentMission = quest.Missions[ActiveQuests.FirstOrDefault(x => x.Id == quest.Id).CurrentMission.Value];

                _currentQuestRewardMoneyText.Value = quest.MoneyReward.ToString();
                _currentQuestTitleCode.OnNext(quest.Id);
                _currentMissionTitleCode.OnNext(quest.Id + "-" + quest
                    .Missions[ActiveQuests.FirstOrDefault(x => x.Id == quest.Id).CurrentMission.Value].MissionId);
                _currentMissionDescriptionCode.OnNext(quest.Id + "-" + quest
                    .Missions[ActiveQuests.FirstOrDefault(x => x.Id == quest.Id).CurrentMission.Value].MissionId + "-d");
                _currentMissionLimitCode.OnNext(currentMission.Limit);
            }
        }

        private void SelectLastSaveViewModel()
        {
            RequestOneSaveClicked(_oneQuestViewModels.Count > 0 ? _oneQuestViewModels[0].QuestId : "");
        }

        public override void Dispose()
        {
            _disposables.Dispose();
        }
        
        //Requests by binders and child viewModels
        #region Requests by binders and child viewModels
        
        public void RequestBackToGame()
        {
            _coreUIManager.TryCloseScreen();
        }
        
        public void RequestSelectInData()
        {
            _gameProxy.TrySelectQuestInData(_selectedQuestId);
        }

        public void RequestOneSaveClicked(string questId)
        {
            SelectOneQuestViewModel(questId);

            if (string.IsNullOrEmpty(questId))
            {
                ShowSaveUINull();
                return;
            }
            
            var quest = QuestsDict[questId];
            ShowSaveUI(quest);
        }
        
        #endregion
        
        private void SelectOneQuestViewModel(string questId)
        {
            _selectedQuestId = questId;
            
            var viewModel = _questsMap.GetValueOrDefault(questId);
            
            _selectedQuestViewModel?.Unselect();
            _selectedQuestViewModel = viewModel;
            _selectedQuestViewModel?.Select();
        }
    }
}
