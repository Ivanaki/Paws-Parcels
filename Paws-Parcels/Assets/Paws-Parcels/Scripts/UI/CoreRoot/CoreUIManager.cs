using System;
using System.Linq;
using Game.Root;
using Messages;
using Paws_Parcels.NPCDialogs;
using Paws_Parcels.Scripts;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Quests;
using Services;
using UI.DialogueScreen;
using UI.Screens.LoadMenu;
using UI.Screens.Loose;
using UI.Screens.Map;
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
        private readonly DialogueService _dialogueService;
        //private readonly QuestsServiceForUi _questsServiceForUi;
        private readonly MessageSystem _messageSystem;
        private readonly LocalizationService _localizationService;
        private readonly QuestsDatabase _questsDatabase;
        private readonly GameProxy _gameProxy;
        private readonly ResourcesService _resourcesService;

        private bool _banOpen = false;
        
        public CoreUIManager(UICoreRootViewModel coreRootViewModel, CoreSignals coreSignals, 
            SettingsService settingsService, LoadService loadService, CursorService cursorService,
            InputsCoreService inputsCoreService, EditorDataService editorDataService,
            DialogueService dialogueService,
            MessageSystem messageSystem, LocalizationService localizationService,
            QuestsDatabase questsDatabase, GameProxy gameProxy, ResourcesService resourcesService)
        {
            _coreRootViewModel = coreRootViewModel;
            _coreSignals = coreSignals;
            _settingsService = settingsService;
            _loadService = loadService;
            _cursorService = cursorService;
            _inputsCoreService = inputsCoreService;
            _editorDataService = editorDataService;
            _dialogueService = dialogueService;
            _messageSystem = messageSystem;
            _localizationService = localizationService;
            _questsDatabase = questsDatabase;
            _gameProxy = gameProxy;
            _resourcesService = resourcesService;
        }

        public void OpenScreenDialog()
        {
            if(_banOpen) return;
            
            var viewModel = new ScreenDialogueViewModel(_dialogueService);
            _coreRootViewModel.OpenScreen(viewModel);
            
            _cursorService.SetCursorStateLocked(false);
            _inputsCoreService.StopAllUiOpened();
            Time.timeScale = 1f;
            _messageSystem.SendCompositeMessage(this, "open_screen_dialog");
        }
        
        public void OpenScreenMap()
        {
            if(_banOpen) return;
            
            var viewModel = new ScreenMapViewModel(this);
            _coreRootViewModel.OpenScreen(viewModel);
            
            _cursorService.SetCursorStateLocked(false);
            _inputsCoreService.NotDisableMovementUiOpened();
            Time.timeScale = 0f;
            
            _messageSystem.SendCompositeMessage(this, "open_screen_map");
        }

        public void CloseScreenMap()
        {
            if(_banOpen) return;
            
            if (_coreRootViewModel.OpenedScreen.CurrentValue is ScreenMapViewModel)
            {
                TryCloseScreen();
            }
        }
        
        public void OpenScreenPause()
        {
            if(_banOpen) return;
            
            var viewModel = new ScreenPauseViewModel(this, _coreSignals.MainMenuSignal, _loadService);
            _coreRootViewModel.OpenScreen(viewModel);
            
            _cursorService.SetCursorStateLocked(false);
            _inputsCoreService.StopAllUiOpened();
            Time.timeScale = 0f;
            
            _messageSystem.SendCompositeMessage(this, "open_screen_pause");
        }

        public void OpenScreenLoose()
        {
            if (_coreRootViewModel.OpenedScreen.CurrentValue is ScreenLooseViewModel)
            {
                return;
            }

            _coreRootViewModel.CloseAllPopups();
            _coreRootViewModel.OpenScreen(new ScreenLooseViewModel(this, _coreSignals.MainMenuSignal,
                _coreSignals.NewGameSignal));

            _cursorService.SetCursorStateLocked(false);
            _inputsCoreService.StopAllUiOpened();
            _inputsCoreService.NotDisableMovementUiClosed();
            Time.timeScale = 0f;
            _banOpen = true;

            _messageSystem.SendCompositeMessage(this, "open_screen_loose");
        }
        
        public void OpenScreenQuests()
        {
            if(_banOpen) return;
            
            var viewModel = new ScreenQuestsViewModel(this, _localizationService, _questsDatabase, _gameProxy, _resourcesService);
            _coreRootViewModel.OpenScreen(viewModel);
            
            _cursorService.SetCursorStateLocked(false);
            _inputsCoreService.NotDisableMovementUiOpened();
            Time.timeScale = 0f;
            
            _messageSystem.SendCompositeMessage(this, "open_screen_quests");
        }

        public bool TryCloseScreen()
        {
            if(_banOpen) return false;

            var closeRes = _coreRootViewModel.TryCloseScreen();

            if(closeRes == false) return false;

            _cursorService.SetCursorStateLocked(true);
            _inputsCoreService.StopAllUiClosed();
            _inputsCoreService.NotDisableMovementUiClosed();
            Time.timeScale = 1f;
            _messageSystem.SendCompositeMessage(this, "close_screen");
            return true;
        }
        

        public void OpenPopupLoadMenu(bool ignoreBan = false)
        {
            if(_banOpen && ignoreBan == false) return;
            
            var viewModel = new PopupLoadMenuViewModel(_loadService);
            _coreRootViewModel.OpenPopup(viewModel);
            
            _messageSystem.SendCompositeMessage(this, "open_popup_load_menu");
        }
        
        public void OpenPopupSettings()
        {
            if(_banOpen) return;
            
            var viewModel = new PopupSettingsViewModel(_settingsService,
                _editorDataService.EditorSettings.MaxSensitivity,
                _editorDataService.EditorSettings.MaxSensitivityOnSlider);
            _coreRootViewModel.OpenPopup(viewModel);
            
            _messageSystem.SendCompositeMessage(this, "open_popup_settings");
        }

        public bool TryCloseLastPopup()
        {
            return _coreRootViewModel.TryCloseLastPopup();
        }

        public void CloseAll()
        {
            _coreRootViewModel.CloseAllPopups();

            if (_coreRootViewModel.OpenedPopups.Any())
            {
                return;
            }

            TryCloseScreen();
        }

        public void BanOpen()
        {
            _banOpen = true;
        }

        public void UnbanOpen()
        {
            if (_coreRootViewModel.OpenedScreen.CurrentValue != null && _coreRootViewModel.OpenedScreen.CurrentValue.CanClose == false)
            {
                return;
            }

            if (_coreRootViewModel.OpenedPopups.Any(popup => popup.CanClose == false))
            {
                return;
            }

            _banOpen = false;
        }
    }
}
