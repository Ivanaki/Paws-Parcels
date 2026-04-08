using System;
using Paws_Parcels.Scripts.Saves.States.GameState;
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
        public UICoreRootViewModel(HUDService hudService, MessageToUserService messageToUserService,
            ShowableInterfaceService showableInterfaceService, GameProxy gameProxy)
        {
            HudViewModel = new HUDViewModel(hudService, showableInterfaceService, gameProxy);
            MessageViewModel = new MessageViewModel(messageToUserService);
        }
    }
}