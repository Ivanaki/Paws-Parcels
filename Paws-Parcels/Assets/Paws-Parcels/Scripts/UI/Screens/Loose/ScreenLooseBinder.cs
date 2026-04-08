using UI.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Loose
{
    public class ScreenLooseBinder : WindowBinder<ScreenLooseViewModel>
    {
        [SerializeField] private Button _buttonOpenLoadMenu;
        [SerializeField] private Button _buttonNewGame;
        [SerializeField] private Button _buttonReturnToMainMenu;

        private void OnEnable()
        {
            _buttonOpenLoadMenu.onClick.AddListener(OnButtonOpenLoadMenuClicked);
            _buttonNewGame.onClick.AddListener(OnButtonNewGameClicked);
            _buttonReturnToMainMenu.onClick.AddListener(OnButtonReturnToMainMenuClicked);
        }

        private void OnDisable()
        {
            _buttonOpenLoadMenu.onClick.RemoveListener(OnButtonOpenLoadMenuClicked);
            _buttonNewGame.onClick.RemoveListener(OnButtonNewGameClicked);
            _buttonReturnToMainMenu.onClick.RemoveListener(OnButtonReturnToMainMenuClicked);
        }

        private void OnButtonOpenLoadMenuClicked()
        {
            ViewModel.RequestOpenLoadMenu();
        }

        private void OnButtonNewGameClicked()
        {
            ViewModel.RequestNewGame();
        }

        private void OnButtonReturnToMainMenuClicked()
        {
            ViewModel.RequestGoToMainMenu();
        }
    }
}
