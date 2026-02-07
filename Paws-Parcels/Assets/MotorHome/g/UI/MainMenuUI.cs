using MotorHome.Scripts.Installers;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MotorHome.Scripts
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _loadGameButton;
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;
        
        [field: SerializeField] public CoreSaverUI CoreSaverUI { get; private set; }
        
        public Observable<Unit> LoadGamePressed => _loadGamePressed;
        public Observable<Unit> NewGamePressed => _newGamePressed;
        public Observable<Unit> SettingsPressed => _settingsPressed;
        public Observable<Unit> ExitPressed => _exitPressed;
        
        private Subject<Unit> _loadGamePressed = new();
        private Subject<Unit> _newGamePressed = new();
        private Subject<Unit> _settingsPressed = new();
        private Subject<Unit> _exitPressed = new();

        private void Awake()
        {
            Hide();

            _loadGameButton.OnClickAsObservable().Subscribe(_ => _loadGamePressed.OnNext(Unit.Default));
            _newGameButton.OnClickAsObservable().Subscribe(_ => _newGamePressed.OnNext(Unit.Default));
            _settingsButton.OnClickAsObservable().Subscribe(_ => _settingsPressed.OnNext(Unit.Default));
            _exitButton.OnClickAsObservable().Subscribe(_ => _exitPressed.OnNext(Unit.Default));
        }
        
        public void Show()
        {
            _panel.SetActive(true);
        }

        public void Hide()
        {
            _panel.SetActive(false);
        }
    }
}