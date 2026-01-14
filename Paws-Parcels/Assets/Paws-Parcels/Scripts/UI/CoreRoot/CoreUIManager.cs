using System;
using Game.Root;
using Paws_Parcels.Scripts;
using Services;
using UI.Screens.LoadMenu;
using UI.Screens.Pause;
using UI.Screens.Quests;
using UI.Screens.Settings;


namespace UI.Core
{
    public class CoreUIManager
    {
        private readonly UICoreRootViewModel _coreRootViewModel;
        private readonly CoreSignals _coreSignals;
        private readonly SettingsService _settingsService;
        private readonly LoadService _loadService;
        private readonly CursorService _cursorService;
        private readonly GameInputService _gameInputService;

        public CoreUIManager(UICoreRootViewModel coreRootViewModel, CoreSignals coreSignals, 
            SettingsService settingsService, LoadService loadService, CursorService cursorService,
            GameInputService gameInputService)
        {
            _coreRootViewModel = coreRootViewModel;
            _coreSignals = coreSignals;
            _settingsService = settingsService;
            _loadService = loadService;
            _cursorService = cursorService;
            _gameInputService = gameInputService;
        }
        
        
        public void OpenScreenPause()
        {
            var viewModel = new ScreenPauseViewModel(this, _coreSignals.MainMenuSignal, _loadService);
            _coreRootViewModel.OpenScreen(viewModel);
            _cursorService.SetCursorStateLocked(false);
            _gameInputService.InputSystem.Gameplay.Disable();
            _gameInputService.InputSystem.UI.Enable();
        }
        
        public void OpenScreenQuests()
        {
            var viewModel = new ScreenQuestsViewModel(this);
            _coreRootViewModel.OpenScreen(viewModel);
            _cursorService.SetCursorStateLocked(false);
            _gameInputService.InputSystem.Gameplay.Disable();
            _gameInputService.InputSystem.UI.Enable();
        }

        public void CloseScreen()
        {
            _coreRootViewModel.CloseScreen();
            _cursorService.SetCursorStateLocked(true);
            _gameInputService.InputSystem.Gameplay.Enable();
            _gameInputService.InputSystem.UI.Disable();
        }
        

        public void OpenPopupLoadMenu()
        {
            var viewModel = new PopupLoadMenuViewModel(_loadService);
            _coreRootViewModel.OpenPopup(viewModel);
        }
        
        public void OpenPopupSettings()
        {
            var viewModel = new PopupSettingsViewModel(_settingsService);
            _coreRootViewModel.OpenPopup(viewModel);
        }

        public bool TryCloseLastPopup()
        {
            return _coreRootViewModel.TryCloseLastPopup();
        }
    }
}