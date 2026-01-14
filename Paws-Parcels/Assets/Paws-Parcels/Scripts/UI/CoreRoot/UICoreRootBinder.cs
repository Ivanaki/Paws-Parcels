using UI.HUD;
using UI.MVVM;
using UnityEngine;

namespace UI.Core
{
    public class UICoreRootBinder : UIRootBinder
    {
        //здесь биндим все биндеры
        [SerializeField] private HUDBinder _hudBinder;
        [SerializeField] private MessageBinder _messageBinder;
        
        protected override void OnBind(UIRootViewModel rootViewModel)
        {
            var viewModel = rootViewModel as UICoreRootViewModel;
            
            _hudBinder.Bind(viewModel.HudViewModel);
            _messageBinder.Bind(viewModel.MessageViewModel);
        }
    }
}