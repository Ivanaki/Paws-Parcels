using MotorHome.Scripts.Installers;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MotorHome.Scripts
{
    public class CoreUI : MonoBehaviour
    {
        public Subject<Unit> ReturnPressed { get; } = new();
        public Subject<Unit> SavePressed { get; } = new();
        public Subject<Unit> LoadPressed { get; } = new();
        public Subject<Unit> SettingsPressed { get; } = new();
        public Subject<Unit> ToMainMenuPressed { get; } = new();

        [SerializeField] private GameObject _panel;
        
        [SerializeField] private Button _return;
        [SerializeField] private Button _save;
        [SerializeField] private Button _loadSave;
        [SerializeField] private Button _settings;
        [SerializeField] private Button _toMainMenu;

        [field: SerializeField] public CoreSaverUI CoreSaverUI { get; private set;}
        //[field: SerializeField] public QuestsSystemUI QuestsSystemUI { get; private set;}
        [field: SerializeField] public Helper Helper { get; private set;}
        [field: SerializeField] public CompassUI CompassUI { get; private set;}
        [field: SerializeField] public GasUI GasUI { get; private set;}

        [SerializeField] private GameObject _canInteractPanel;

        private void Awake()
        {
            CanInteract(false);
            
            _return.onClick.AddListener(() => ReturnPressed.OnNext(Unit.Default));
            _save.onClick.AddListener(() => SavePressed.OnNext(Unit.Default));
            _loadSave.onClick.AddListener(() => LoadPressed.OnNext(Unit.Default));
            _settings.onClick.AddListener(() => SettingsPressed.OnNext(Unit.Default));
            _toMainMenu.onClick.AddListener(() => ToMainMenuPressed.OnNext(Unit.Default));
        }

        public void Show()
        {
            //print("Show");
            _panel.SetActive(true);
        }

        public void Hide()
        {
            //print("Hide");
            _panel.SetActive(false);
        }

        public void CanInteract(bool isShow)
        {
            _canInteractPanel.SetActive(isShow);
        }
    }
}