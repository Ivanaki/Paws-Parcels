using System.Collections.Generic;
using System.Linq;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MotorHome.Scripts
{
    public class CoreSaverUI : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private RawImage _texture;
        [SerializeField] private TMP_Text _missionName;
        [SerializeField] private TMP_Text _saveTime;
        [SerializeField] private TMP_Text _saveType;
        [SerializeField] private Button _loadButton;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Transform _gridSpawn;
        [SerializeField] private GameLoadPrefab _gameLoadPrefab;
        
        private readonly Subject<Unit> _loadButtonPressed = new();
        public Observable<Unit> LoadButtonPressed => _loadButtonPressed;
        
        private readonly Subject<Unit> _deleteButtonPressed = new();
        public Observable<Unit> DeleteButtonPressed => _deleteButtonPressed;
        
        private readonly Subject<Unit> _exitButtonPressed = new();
        public Observable<Unit> ExitButtonPressed => _exitButtonPressed;
        
        
        private void Awake()
        {
            _loadButton.onClick.AddListener(() => _loadButtonPressed.OnNext(Unit.Default));
            _deleteButton.onClick.AddListener(() => _deleteButtonPressed.OnNext(Unit.Default));
            _exitButton.onClick.AddListener(() => _exitButtonPressed.OnNext(Unit.Default));
        }

        public void Show()
        {
            _panel.SetActive(true);
        }

        public void Hide()
        {
            _panel.SetActive(false);
        }

        public void ShowSelectedSave(CoreSaveData coreSaveData, Texture2D texture)
        {
            _texture.texture = texture;
            _missionName.text = coreSaveData.MissionName;
            _saveTime.text = coreSaveData.Time.ToString("dd.MM.yyyy HH:mm");
            _saveType.text = coreSaveData.SaveType;
        }

        private CompositeDisposable _disposables;
        private GameLoadPrefab _selected;

        private Subject<int> _newSelected = new();
        public Observable<int> NewSelected => _newSelected;
        
        public void UpdateLoading(Dictionary<int, CoreSaveData> saveData)
        {
            _selected?.Deselect();
            
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();
            
            for (int i = _gridSpawn.childCount - 1; i >= 0; i--)
            {
                Destroy(_gridSpawn.GetChild(i).gameObject);
            }
            
            foreach (var coreSaveData in saveData.Reverse())
            {
                var obj = Instantiate(_gameLoadPrefab, _gridSpawn);
                obj.Initialize(coreSaveData.Value.Time.ToString("dd.MM.yyyy HH:mm"), coreSaveData.Key);
                _disposables.Add(obj.Selected.Subscribe(_ =>
                {
                    _selected?.Deselect();
                    obj.Select();
                    _selected = obj;
                    _newSelected.OnNext(_selected.SaveNumber);
                }));
            }
        }

        public void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}