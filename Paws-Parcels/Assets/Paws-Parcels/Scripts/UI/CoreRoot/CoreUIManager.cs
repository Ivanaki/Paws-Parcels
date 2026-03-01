using System;
using Game.Root;
using Paws_Parcels.Scripts;
using Services;
using UI.Screens.LoadMenu;
using UI.Screens.Pause;
using UI.Screens.Quests;
using UI.Screens.Settings;
using UnityEngine;


namespace UI.Core
{
    public class CoreUIManager
    {
        private readonly UICoreRootViewModel _coreRootViewModel;
        private readonly CoreSignals _coreSignals;
        private readonly SettingsService _settingsService;
        private readonly LoadService _loadService;
        private readonly CursorService _cursorService;
        private readonly InputsCoreService _inputsCoreService;
        private readonly EditorDataService _editorDataService;

        public CoreUIManager(UICoreRootViewModel coreRootViewModel, CoreSignals coreSignals, 
            SettingsService settingsService, LoadService loadService, CursorService cursorService,
            InputsCoreService inputsCoreService, EditorDataService editorDataService)
        {
            _coreRootViewModel = coreRootViewModel;
            _coreSignals = coreSignals;
            _settingsService = settingsService;
            _loadService = loadService;
            _cursorService = cursorService;
            _inputsCoreService = inputsCoreService;
            _editorDataService = editorDataService;
        }
        
        
        public void OpenScreenPause()
        {
            var viewModel = new ScreenPauseViewModel(this, _coreSignals.MainMenuSignal, _loadService);
            _coreRootViewModel.OpenScreen(viewModel);
            
            _cursorService.SetCursorStateLocked(false);
            _inputsCoreService.StopAllUiOpened();
            Time.timeScale = 0f;
        }
        
        public void OpenScreenQuests()
        {
            var viewModel = new ScreenQuestsViewModel(this);
            _coreRootViewModel.OpenScreen(viewModel);
            
            _cursorService.SetCursorStateLocked(false);
            _inputsCoreService.NotDisableMovementUiOpened();
            Time.timeScale = 1f;
        }

        public void CloseScreen()
        {
            _coreRootViewModel.CloseScreen();
            
            _cursorService.SetCursorStateLocked(true);
            _inputsCoreService.StopAllUiClosed();
            _inputsCoreService.NotDisableMovementUiClosed();
            Time.timeScale = 1f;
        }
        

        public void OpenPopupLoadMenu()
        {
            var viewModel = new PopupLoadMenuViewModel(_loadService);
            _coreRootViewModel.OpenPopup(viewModel);
        }
        
        public void OpenPopupSettings()
        {
            var viewModel = new PopupSettingsViewModel(_settingsService,
                _editorDataService.EditorSettings.MaxSensitivity,
                _editorDataService.EditorSettings.MaxSensitivityOnSlider);
            _coreRootViewModel.OpenPopup(viewModel);
        }

        public bool TryCloseLastPopup()
        {
            return _coreRootViewModel.TryCloseLastPopup();
        }
    }
}