using UI.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Map
{
    public class ScreenMapBinder : WindowBinder<ScreenMapViewModel>
    {
        [SerializeField] private Button _buttonBack;
        
        private void OnEnable()
        {
            _buttonBack.onClick.AddListener(OnButtonBackClicked);
        }

        private void OnDisable()
        {
            _buttonBack.onClick.RemoveListener(OnButtonBackClicked);
        }

        private void OnButtonBackClicked()
        {
            ViewModel.RequestBackToGame();
        }
    }
}