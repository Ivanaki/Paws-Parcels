using Game.Root;
using Paws_Parcels.Scripts;
using R3;
using UI.Core;
using UI.Screens;
using UI.Screens.LoadMenu;
using UI.Screens.MainMenu;
using UI.Screens.Pause;
using UI.Screens.Quests;
using UI.Screens.Settings;

namespace UI.MainMenu
{
    public class MainMenuUIManager
    {
        private readonly UIMainMenuRootViewModel _mainMenuRootViewModel;
        private readonly MainMenuSignals _mainMenuSignals;
        private readonly LoadService _loadService;
        private readonly SettingsService _settingsService;

        public MainMenuUIManager(UIMainMenuRootViewModel mainMenuRootViewModel,
            MainMenuSignals mainMenuSignals, LoadService loadService, SettingsService settingsService)
        {
            _mainMenuRootViewModel = mainMenuRootViewModel;
            _mainMenuSignals = mainMenuSignals;
            _loadService = loadService;
            _settingsService = settingsService;
        }
        
        public void OpenScreenMainMenu()
        {
            var viewModel = new ScreenMainMenuViewModel(this, _mainMenuSignals);
            _mainMenuRootViewModel.OpenScreen(viewModel);
        }

        public void CloseScreen()
        {
            _mainMenuRootViewModel.CloseScreen();
        }
        
        public void OpenPopupSettings()
        {
            var viewModel = new PopupSettingsViewModel(_settingsService);
            _mainMenuRootViewModel.OpenPopup(viewModel);
        }
        
        public void OpenPopupLoadMenu()
        {
            var viewModel = new PopupLoadMenuViewModel(_loadService);
            _mainMenuRootViewModel.OpenPopup(viewModel);
        }
    }
}