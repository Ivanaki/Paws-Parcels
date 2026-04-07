using UI.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Pause
{
    public class ScreenPauseBinder : WindowBinder<ScreenPauseViewModel>
    {
        [SerializeField] private Button _buttonBack;
        [SerializeField] private Button _buttonStuck;
        [SerializeField] private Button _buttonSave;
        [SerializeField] private Button _buttonOpenLoadMenu;
        [SerializeField] private Button _buttonOpenSettings;
        [SerializeField] private Button _buttonReturnToMainMenu;

        private void OnEnable()
        {
            _buttonBack.onClick.AddListener(OnButtonBackClicked);
            _buttonStuck.onClick.AddListener(OnButtonStuckClicked);
            _buttonSave.onClick.AddListener(OnButtonSaveClicked);
            _buttonOpenLoadMenu.onClick.AddListener(OnButtonOpenLoadMenuClicked);
            _buttonOpenSettings.onClick.AddListener(OnButtonOpenSettingsClicked);
            _buttonReturnToMainMenu.onClick.AddListener(OnButtonReturnToMainMenuClicked);
        }

        private void OnDisable()
        {
            _buttonBack.onClick.RemoveListener(OnButtonBackClicked);
            _buttonStuck.onClick.RemoveListener(OnButtonStuckClicked);
            _buttonSave.onClick.RemoveListener(OnButtonSaveClicked);
            _buttonOpenLoadMenu.onClick.RemoveListener(OnButtonOpenLoadMenuClicked);
            _buttonOpenSettings.onClick.RemoveListener(OnButtonOpenSettingsClicked);
            _buttonReturnToMainMenu.onClick.RemoveListener(OnButtonReturnToMainMenuClicked);
        }

        private void OnButtonBackClicked()
        {
            ViewModel.RequestBackToGame();
        }

        private void OnButtonStuckClicked()
        {
            ViewModel.RequestStuck();
        }

        private void OnButtonSaveClicked()
        {
            ViewModel.RequestSave();
        }
        
        private void OnButtonOpenLoadMenuClicked()
        {
            ViewModel.RequestOpenLoadMenu();
        }
        
        private void OnButtonOpenSettingsClicked()
        {
            ViewModel.RequestOpenSettings();
        }

        private void OnButtonReturnToMainMenuClicked()
        {
            ViewModel.RequestGoToMainMenu();
        }
    }
}