using R3;
using UI.Core;
using UI.MVVM;

namespace UI.Screens.Pause
{
    public class ScreenPauseViewModel : WindowViewModel
    {
        private readonly CoreUIManager _coreUIManager;
        private readonly Subject<Unit> _exitToMainMenuRequest;
        public override string Id => "Pause";

        public ScreenPauseViewModel(CoreUIManager coreUIManager, Subject<Unit> exitToMainMenuRequest)
        {
            _coreUIManager = coreUIManager;
            _exitToMainMenuRequest = exitToMainMenuRequest;
        }

        public void RequestBackToGame()
        {
            
        }

        public void RequestSave()
        {
            
        }

        public void RequestOpenLoadMenu()
        {
            //_coreUIManager.OpenScreenLoadMenu();
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