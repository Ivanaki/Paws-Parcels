using Game.Root;
using Paws_Parcels.Scripts;
using R3;
using UI.Core;
using UI.MainMenu;
using UI.MVVM;

namespace UI.Screens.MainMenu
{
    public class ScreenMainMenuViewModel : WindowViewModel
    {
        private readonly MainMenuUIManager _mainMenuUIManager;
        private readonly MainMenuSignals _mainMenuSignals;
        private readonly LoadService _loadService;

        public override string Id => "MainMenu";

        public ScreenMainMenuViewModel(MainMenuUIManager mainMenuUIManager,
            MainMenuSignals mainMenuSignals, LoadService loadService)
        {
            _mainMenuUIManager = mainMenuUIManager;
            _mainMenuSignals = mainMenuSignals;
            _loadService = loadService;
        }

        public void RequestOpenLoadMenu()
        {
            
        }
        
        public void RequestNewGame()
        {
            _mainMenuSignals.StartNewGame();
        }

        public void RequestOpenSettings()
        {
            _mainMenuUIManager.OpenPopupSettings();
        }

        public void RequestQuit()
        {
            _mainMenuSignals.Quit();
        }
    }
}