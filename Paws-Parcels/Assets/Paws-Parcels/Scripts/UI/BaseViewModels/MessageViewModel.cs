using R3;
using Services;
using UnityEngine;

namespace UI.HUD
{
    public class MessageViewModel
    {
        public ReadOnlyReactiveProperty<string> MessageToMainMenu { get; }
        public ReadOnlyReactiveProperty<Color> MessageToMainMenuColor { get; }

        public MessageViewModel(MessageToUserService messageToUserService)
        {
            MessageToMainMenu = messageToUserService.MessageToMainMenu;
            MessageToMainMenuColor = messageToUserService.MessageToMainMenuColor;
        }
    }
}