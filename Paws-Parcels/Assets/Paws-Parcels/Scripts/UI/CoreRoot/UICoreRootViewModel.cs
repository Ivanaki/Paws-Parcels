using System;
using Services;
using UI.HUD;
using UI.MVVM;

namespace UI.Core
{
    public class UICoreRootViewModel : UIRootViewModel
    {
        public readonly HUDViewModel HudViewModel;
        public readonly MessageViewModel MessageViewModel;
        
        //сюда кидаем сервисы и создаем ViewModels с этими сервисами
        public UICoreRootViewModel(HUDService hudService, MessageToUserService messageToUserService)
        {
            HudViewModel = new HUDViewModel(hudService);
            MessageViewModel = new MessageViewModel(messageToUserService);
        }
    }
}