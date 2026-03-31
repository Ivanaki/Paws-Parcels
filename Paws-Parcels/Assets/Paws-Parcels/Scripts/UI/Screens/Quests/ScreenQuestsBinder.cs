using UI.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Quests
{
    public class ScreenQuestsBinder : WindowBinder<ScreenQuestsViewModel>
    {
        [SerializeField] private Button _exitButton;
        
        private void OnEnable()
        {
            _exitButton.onClick.AddListener(OnButtonBackClicked);
        }

        private void OnDisable()
        {
            _exitButton.onClick.RemoveListener(OnButtonBackClicked);
        }
        
        private void OnButtonBackClicked()
        {
            ViewModel.RequestBackToGame();
        }
    }
}