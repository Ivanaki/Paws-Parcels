using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MotorHome.Scripts
{
    public class Helper : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _destination;
        [SerializeField] private TMP_Text _cargo;
        [SerializeField] private TMP_Text _money;
        [SerializeField] private Button _selectButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Transform _gridSpawn;
        [SerializeField] private GameQuestsPrefab _gameQuestsPrefab;
        
        public Observable<QuestEnum> SelectButtonPressed => _selectButtonPressed;
        public Observable<Unit> ReturnButtonPressed => _returnButtonPressed;
        
        private readonly Subject<Unit> _returnButtonPressed = new();
        private readonly Subject<QuestEnum> _selectButtonPressed = new();
        
        private IDisposable _disposable;
        
        [SerializeField] private TMP_Text _text;
        

        
        public void Inject(UIActiveCoreManager uiActiveCoreManager)
        {
            _exitButton.OnClickAsObservable().Subscribe(_ => _returnButtonPressed.OnNext(Unit.Default));
            _selectButton.OnClickAsObservable().Subscribe(_ =>
                _selectButtonPressed.OnNext(_selected == null ? QuestEnum.None : _selected.Quest));
            
            _disposable?.Dispose();
            _disposable = uiActiveCoreManager.CurrentUI.Subscribe(value =>
            {
                if (value == UIEnum.QuestsPanel)
                {
                    _panel.SetActive(true);
                }

                if (value == UIEnum.Game)
                {
                    _panel.SetActive(false);
                }
            });
        }
        
        
        
        public void UpdateQuests(List<(QuestData, QuestDataMini)> questDatas)
        {
            _selected?.Deselect();
            
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();
            
            for (int i = _gridSpawn.childCount - 1; i >= 0; i--)
            {
                Destroy(_gridSpawn.GetChild(i).gameObject);
            }

            for (var index = 0; index < questDatas.Count; index++)
            {
                var coreSaveData = questDatas[index];
                var obj = Instantiate(_gameQuestsPrefab, _gridSpawn);
                obj.Initialize(coreSaveData.Item1.MainName, coreSaveData.Item1.Quest);
                
                if (index == 0)
                {
                    obj.Select();
                    _selected = obj;
                    ShowSelectedQuest(coreSaveData.Item1, coreSaveData.Item2);
                }
                
                _disposables.Add(obj.Selected.Subscribe(_ =>
                {
                    _selected?.Deselect();
                    obj.Select();
                    _selected = obj;
                    _newSelected.Value = _selected.Quest;
                }));
            }

            if (questDatas.Count == 0)
            {
                _newSelected.OnNext(QuestEnum.None);
            }
        }
        
        public void ShowSelectedQuest(QuestData questData, QuestDataMini questDataMini)
        {
            _text.text = questData.MainName;
            
            _name.text = questData.MainName;
            _description.text = questData.Description;
            _destination.text = questData.Destination;
            _cargo.text = questData.Cargo;
            _money.text = questDataMini.Money.ToString();
        }

        private CompositeDisposable _disposables;
        private GameQuestsPrefab _selected;

        private ReactiveProperty<QuestEnum> _newSelected = new(QuestEnum.None);
        public ReadOnlyReactiveProperty<QuestEnum> NewSelected => _newSelected;

        public void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}