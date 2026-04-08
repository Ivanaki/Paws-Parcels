using System;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Quests;
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
            ShowableInterfaceService showableInterfaceService, GameProxy gameProxy,
            LocalizationService localizationService, QuestsDatabase questsDatabase)
        {
            HudViewModel = new HUDViewModel(hudService, showableInterfaceService, gameProxy,
                localizationService, questsDatabase);
            MessageViewModel = new MessageViewModel(messageToUserService);
        }

        public override void Dispose()
        {
            HudViewModel.Dispose();
            base.Dispose();
        }
    }
}
