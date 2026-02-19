using System;
using System.Collections.Generic;
using System.Linq;
using MotorHome.Scripts.Saves;
using MotorHome.Scripts.ScriptableObjects;
using R3;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class QuestsSystem : IInitializable, IDisposable
    {
        private readonly QuestsSO _questsSo;
        private readonly QuestsPanel _questsPanel;
        private readonly CoreUI _coreUI;
        private readonly LanguageChanger _languageChanger;
        private readonly GameStateProvider _gameStateProvider;
        private readonly CompassUI _compassUI;
        private readonly QuestTargets _questTargets;
        private readonly Dictionary<QuestEnum, QuestDataMini> _questsMoney = new();
        private List<QuestEnum> _currentQuests = new();
        
        private CompositeDisposable _disposable;
        
        
        public QuestsSystem(QuestsSO questsSo, QuestsPanel questsPanel, CoreUI coreUI,
            LanguageChanger languageChanger, GameStateProvider gameStateProvider, 
            QuestTargets questTargets)
        {
            _questsSo = questsSo;
            _questsPanel = questsPanel;
            _coreUI = coreUI;
            _languageChanger = languageChanger;
            _gameStateProvider = gameStateProvider;
            _compassUI = coreUI.CompassUI;
            _questTargets = questTargets;
        }

        public void NewQuest(QuestEnum questKey)
        {
            var a = _languageChanger.GetQuestData(questKey);
            
            if (a.Item1)
            {
                _currentQuests.Add(questKey);
                UpdateData();
            }
            else
            {
                Debug.LogError("No quest found for key: " + questKey);
            }
        }

        public void CompleteQuest(QuestEnum questKey)
        {
            var a = _languageChanger.GetQuestData(questKey);
            if (a.Item1)
            {
                if (_currentQuests.Contains(questKey))
                {
                    _currentQuests.Remove(questKey);
                    UpdateData();
                }
                else
                {
                    //Debug.LogError("This quest is not active to complete its: " + questKey);
                }
            }
            else
            {
                //Debug.LogError("No quest found for key: " + questKey);
            }
        }

        private QuestEnum _compassSelected = QuestEnum.None;
        
        public void Initialize()
        {
            _disposable = new CompositeDisposable();
            IDisposable dis;
            
            foreach (var questsSoQuest in _questsSo.Quests)
            {
                _questsMoney.Add(questsSoQuest.Quest, questsSoQuest);
            }
            
            dis = _coreUI.Helper.NewSelected.Subscribe(ShowSelectedQuest);
            _disposable.Add(dis);
            dis = _coreUI.Helper.SelectButtonPressed.Subscribe(value =>
            {
                Debug.Log("SelectButtonPressed");
                _compassSelected = _coreUI.Helper.NewSelected.CurrentValue;
                _compassUI.SetTarget(_questTargets.GetQuestTarget(value));
                
            }); _disposable.Add(dis);

            dis = _languageChanger.OnLanguageChanged.Subscribe(_ =>
            {
                UpdateData();
            }); _disposable.Add(dis);

            dis = _gameStateProvider.Saved.Subscribe(_ =>
            {
                _gameStateProvider.CoreState.CompassSelected = _compassSelected;
                _gameStateProvider.CoreState.Quests = _currentQuests.ToArray();
            });_disposable.Add(dis);
            
            dis =_gameStateProvider.Updated.Subscribe(_ =>
            {
                _compassUI.SetTarget(_questTargets.GetQuestTarget(_gameStateProvider.CoreState.CompassSelected));
                
                if (_gameStateProvider.CoreState.Quests != null)
                {
                    _currentQuests = _gameStateProvider.CoreState.Quests.ToList();
                }
                UpdateData();
            });_disposable.Add(dis);
        }

        private void ShowSelectedQuest(QuestEnum questEnum)
        {
            var a = _languageChanger.GetQuestData(questEnum);
            if (a.Item1)
            {
                var t = _questsMoney.TryGetValue(questEnum, out var questDataMini);
                if (!t)
                {
                    Debug.LogError($"Quest {questEnum} is not in scriptable object");
                    return;
                }
                _coreUI.Helper.ShowSelectedQuest(a.Item2, questDataMini);
            }
            else
            {
                _coreUI.Helper.ShowSelectedQuest(new QuestData(QuestEnum.None, "-",
                    "-", "-", "-"), new QuestDataMini(QuestEnum.None, 0));
            }
        }

        private void UpdateData()
        {
            var t = _currentQuests.Select(x => 
                    (_languageChanger.GetQuestData(x), _questsMoney[x]))
                .Select(x => (x.Item1.Item2,x.Item2)).ToList();
            _coreUI.Helper.UpdateQuests(t);
        }
        
        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}