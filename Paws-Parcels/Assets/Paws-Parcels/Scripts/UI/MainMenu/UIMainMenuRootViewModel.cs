using Services;
using UI.HUD;
using UI.MVVM;

namespace UI.MainMenu
{
    public class UIMainMenuRootViewModel : UIRootViewModel
    {
        public MessageViewModel MessageViewModel { get; }

        public UIMainMenuRootViewModel(MessageToUserService messageToUserService)
        {
            MessageViewModel = new MessageViewModel(messageToUserService);
        }
    }
}