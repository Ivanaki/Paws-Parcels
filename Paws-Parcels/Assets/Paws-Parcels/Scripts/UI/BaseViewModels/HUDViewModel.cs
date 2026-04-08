using System;
using System.Collections.Generic;
using System.Linq;
using ObservableCollections;
using Paws_Parcels.Scripts.Quests;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Quests;
using R3;
using Services;

namespace UI.HUD
{
    public class HUDViewModel : IDisposable
    {
        private const string NoneText = "";

        //public ObservableHashSet<ShowableInUIBehaviour> ObjectsToShow { get; }
        //public IReadOnlyDictionary<string, UiObject> ObjectsPrefabsDict { get; }
        public ReadOnlyReactiveProperty<bool> IsCompassActive { get; }
        public ReadOnlyReactiveProperty<bool> ActiveHud { get; }
        
        public ReadOnlyReactiveProperty<int> Money { get; }
        public ReadOnlyReactiveProperty<string> CurrentQuestTitleText => _currentQuestTitleText;
        public ReadOnlyReactiveProperty<string> CurrentMissionTitleText => _currentMissionTitleText;
        
        public GameProxy GameProxy { get; }

        private readonly LocalizationService _localizationService;
        private readonly IReadOnlyDictionary<string, Quest> _questsDict;
        private readonly ReactiveProperty<string> _currentQuestTitleText = new(NoneText);
        private readonly ReactiveProperty<string> _currentMissionTitleText = new(NoneText);
        private readonly Dictionary<QuestDataProxy, IDisposable> _questSubscriptions = new();
        private readonly CompositeDisposable _disposables = new();
        
        public HUDViewModel(HUDService hudService, ShowableInterfaceService showableInterfaceService,
            GameProxy gameProxy, LocalizationService localizationService, QuestsDatabase questsDatabase)
        {
            //ObjectsToShow = objectsToShow;
            //ObjectsPrefabsDict = objectsPrefabsDict;
            ActiveHud = showableInterfaceService.HudActive;
            IsCompassActive = hudService.IsCompassActive;
            Money = hudService.Money;
            GameProxy = gameProxy;
            _localizationService = localizationService;
            _questsDict = questsDatabase.QuestsDict;

            foreach (var activeQuest in GameProxy.ActiveQuests)
            {
                SubscribeQuest(activeQuest);
            }

            _disposables.Add(GameProxy.SelectedQuest.Subscribe(_ => UpdateCurrentQuestInfo()));
            _disposables.Add(GameProxy.ActiveQuests.ObserveAdd().Subscribe(e =>
            {
                SubscribeQuest(e.Value);
                UpdateCurrentQuestInfo();
            }));
            _disposables.Add(GameProxy.ActiveQuests.ObserveRemove().Subscribe(e =>
            {
                UnsubscribeQuest(e.Value);
                UpdateCurrentQuestInfo();
            }));
            _disposables.Add(GameProxy.ActiveQuests.ObserveClear().Subscribe(_ =>
            {
                foreach (var subscription in _questSubscriptions.Values)
                {
                    subscription.Dispose();
                }

                _questSubscriptions.Clear();
                UpdateCurrentQuestInfo();
            }));
            _disposables.Add(_localizationService.CurrentLocale.Skip(1).Subscribe(_ => UpdateCurrentQuestInfo()));

            UpdateCurrentQuestInfo();
        }

        public void Dispose()
        {
            foreach (var subscription in _questSubscriptions.Values)
            {
                subscription.Dispose();
            }

            _questSubscriptions.Clear();
            _disposables.Dispose();
        }

        private void SubscribeQuest(QuestDataProxy questDataProxy)
        {
            if (_questSubscriptions.ContainsKey(questDataProxy))
            {
                return;
            }

            var subscription = questDataProxy.CurrentMission.Subscribe(_ => UpdateCurrentQuestInfo());
            _questSubscriptions.Add(questDataProxy, subscription);
        }

        private void UnsubscribeQuest(QuestDataProxy questDataProxy)
        {
            if (_questSubscriptions.TryGetValue(questDataProxy, out var subscription))
            {
                subscription.Dispose();
                _questSubscriptions.Remove(questDataProxy);
            }
        }

        private void UpdateCurrentQuestInfo()
        {
            var selectedQuestId = GameProxy.SelectedQuest.CurrentValue;
            var currentQuest = GameProxy.ActiveQuests.FirstOrDefault(q => q.Id == selectedQuestId)
                ?? GameProxy.ActiveQuests.FirstOrDefault();

            if (currentQuest == null)
            {
                _currentQuestTitleText.Value = NoneText;
                _currentMissionTitleText.Value = NoneText;
                return;
            }

            if (_questsDict.TryGetValue(currentQuest.Id, out var quest) == false)
            {
                _currentQuestTitleText.Value = NoneText;
                _currentMissionTitleText.Value = NoneText;
                return;
            }

            _currentQuestTitleText.Value =
                _localizationService.GetCurrentLanguageStringOrDefault(quest.Id, Tables.Quests);

            var missionIndex = currentQuest.CurrentMission.CurrentValue;
            if (missionIndex < 0 || missionIndex >= quest.Missions.Count)
            {
                _currentMissionTitleText.Value = NoneText;
                return;
            }

            var missionId = quest.Missions[missionIndex].MissionId;
            _currentMissionTitleText.Value =
                _localizationService.GetCurrentLanguageStringOrDefault($"{quest.Id}-{missionId}", Tables.Quests);
        }
    }
}
