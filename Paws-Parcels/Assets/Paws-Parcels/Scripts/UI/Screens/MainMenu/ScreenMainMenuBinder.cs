using UI.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.MainMenu
{
    public class ScreenMainMenuBinder : WindowBinder<ScreenMainMenuViewModel>
    {
        [SerializeField] private Button _buttonOpenLoadMenu;
        [SerializeField] private Button _buttonNewGame;
        [SerializeField] private Button _buttonOpenSettings;
        [SerializeField] private Button _buttonQuit;

        private void OnEnable()
        {
            _buttonOpenLoadMenu.onClick.AddListener(OnButtonOpenLoadMenuClicked);
            _buttonNewGame.onClick.AddListener(OnButtonNewGameClicked);
            _buttonOpenSettings.onClick.AddListener(OnButtonOpenSettingsClicked);
            _buttonQuit.onClick.AddListener(OnButtonQuitClicked);
        }

        private void OnDisable()
        {
            _buttonOpenLoadMenu.onClick.RemoveListener(OnButtonOpenLoadMenuClicked);
            _buttonNewGame.onClick.RemoveListener(OnButtonNewGameClicked);
            _buttonOpenSettings.onClick.RemoveListener(OnButtonOpenSettingsClicked);
            _buttonQuit.onClick.RemoveListener(OnButtonQuitClicked);
        }

        private void OnButtonOpenLoadMenuClicked()
        {
            ViewModel.RequestOpenLoadMenu();
        }

        private void OnButtonNewGameClicked()
        {
            ViewModel.RequestNewGame();
        }
        
        private void OnButtonOpenSettingsClicked()
        {
            ViewModel.RequestOpenSettings();
        }
        
        private void OnButtonQuitClicked()
        {
            ViewModel.RequestQuit();
        }
    }
}