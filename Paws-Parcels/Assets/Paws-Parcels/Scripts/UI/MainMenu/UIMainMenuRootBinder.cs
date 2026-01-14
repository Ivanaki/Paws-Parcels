using UI.HUD;
using UI.MVVM;
using UnityEngine;

namespace UI.MainMenu
{
    public class UIMainMenuRootBinder : UIRootBinder
    {
        [SerializeField] private MessageBinder _messageBinder;
        
        protected override void OnBind(UIRootViewModel rootViewModel)
        {
            var viewModel = rootViewModel as UIMainMenuRootViewModel;
            
            _messageBinder.Bind(viewModel.MessageViewModel);
        }
    }
}