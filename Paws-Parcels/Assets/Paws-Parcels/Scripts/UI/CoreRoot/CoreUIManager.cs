using Game.Root;
using UI.Screens.LoadMenu;
using UI.Screens.Pause;
using UI.Screens.Quests;
using UI.Screens.Settings;


namespace UI.Core
{
    public class CoreUIManager
    {
        private readonly UICoreRootViewModel _coreRootViewModel;
        private readonly CoreScenesChanger _coreScenesChanger;
        private readonly SettingsService _settingsService;

        public CoreUIManager(UICoreRootViewModel coreRootViewModel, CoreScenesChanger coreScenesChanger, SettingsService settingsService)
        {
            _coreRootViewModel = coreRootViewModel;
            _coreScenesChanger = coreScenesChanger;
            _settingsService = settingsService;
        }
        
        
        public void OpenScreenPause()
        {
            var viewModel = new ScreenPauseViewModel(this, _coreScenesChanger.MainMenuSignal);
            _coreRootViewModel.OpenScreen(viewModel);
        }
        
        public void OpenScreenQuests()
        {
            var viewModel = new ScreenQuestsViewModel(this);
            _coreRootViewModel.OpenScreen(viewModel);
        }

        public void CloseScreen()
        {
            _coreRootViewModel.CloseScreen();
        }
        

        public void OpenPopupLoadMenu()
        {
            var viewModel = new PopupLoadMenuViewModel();
            _coreRootViewModel.OpenPopup(viewModel);
        }
        
        public void OpenPopupSettings()
        {
            var viewModel = new PopupSettingsViewModel(_settingsService);
            _coreRootViewModel.OpenPopup(viewModel);
        }
        
        /*public PopupAViewModel OpenPopupA()
        {
            var a = new PopupAViewModel();
            var rootUI = Cntainer.Resolve<UIGameplayRootViewModel>();

            rootUI.OpenPopup(a);

            return a;
        }

        public PopupBViewModel OpenPopupB()
        {
            var b = new PopupBViewModel();
            var rootUI = Contaier.Resolve<UIGameplayRootViewModel>();
            rootUI.OpenPopup(b);

            return b;
        }*/
    }
}