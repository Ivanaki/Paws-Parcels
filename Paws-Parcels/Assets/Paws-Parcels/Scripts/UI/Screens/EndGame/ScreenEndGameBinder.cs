using UI.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.EndGame
{
    public class ScreenEndGameBinder : WindowBinder<ScreenEndGameViewModel>
    {
        [SerializeField] private Button _buttonContinue;
        [SerializeField] private Button _buttonReturnToMainMenu;
        [SerializeField] private string _surveyUrl;

        private void OnEnable()
        {
            _buttonContinue.onClick.AddListener(OnButtonContinueClicked);
            _buttonReturnToMainMenu.onClick.AddListener(OnButtonReturnToMainMenuClicked);
        }

        private void OnDisable()
        {
            _buttonContinue.onClick.RemoveListener(OnButtonContinueClicked);
            _buttonReturnToMainMenu.onClick.RemoveListener(OnButtonReturnToMainMenuClicked);
        }
        
        private void OnButtonContinueClicked()
        {
            OpenSurveyUrl();
            ViewModel.RequestContinue();
        }

        private void OnButtonReturnToMainMenuClicked()
        {
            OpenSurveyUrl();
            ViewModel.RequestGoToMainMenu();
        }

        private void OpenSurveyUrl()
        {
            if (string.IsNullOrWhiteSpace(_surveyUrl))
            {
                Debug.LogWarning("End game survey URL is not configured.");
                return;
            }

            Application.OpenURL(_surveyUrl);
        }
    }
}
