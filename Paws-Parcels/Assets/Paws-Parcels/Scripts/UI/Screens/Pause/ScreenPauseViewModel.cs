using Paws_Parcels.Scripts;
using R3;
using UI.Core;
using UI.MVVM;

namespace UI.Screens.Pause
{
    public class ScreenPauseViewModel : WindowViewModel
    {
        private readonly CoreUIManager _coreUIManager;
        private readonly Subject<Unit> _exitToMainMenuRequest;
        private readonly LoadService _loadService;
        public override string Id => "Pause";

        public ScreenPauseViewModel(CoreUIManager coreUIManager, Subject<Unit> exitToMainMenuRequest, LoadService loadService)
        {
            _coreUIManager = coreUIManager;
            _exitToMainMenuRequest = exitToMainMenuRequest;
            _loadService = loadService;
        }

        public void RequestBackToGame()
        {
            _coreUIManager.TryCloseScreen();
        }

        public void RequestSave()
        {
            _loadService.SaveGame();
        }

        public void RequestStuck()
        {
            _loadService.Stuck();
        }

        public void RequestOpenLoadMenu()
        {
            _coreUIManager.OpenPopupLoadMenu();
        }

        public void RequestOpenSettings()
        {
            _coreUIManager.OpenPopupSettings();
        }

        public void RequestGoToMainMenu()
        {
            _exitToMainMenuRequest.OnNext(Unit.Default);
        }
    }
}