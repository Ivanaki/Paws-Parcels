using R3;
using UI.Core;
using UI.MVVM;

namespace UI.Screens.EndGame
{
    public class ScreenEndGameViewModel : WindowViewModel
    {
        private readonly CoreUIManager _coreUIManager;
        private readonly Subject<Unit> _exitToMainMenuRequest;

        public override string Id => "EndGame";
        public override bool CanClose => true;

        public ScreenEndGameViewModel(CoreUIManager coreUIManager, Subject<Unit> exitToMainMenuRequest)
        {
            _coreUIManager = coreUIManager;
            _exitToMainMenuRequest = exitToMainMenuRequest;
        }

        public void RequestContinue()
        {
            _coreUIManager.CloseEndGameScreenAndContinue();
        }

        public void RequestGoToMainMenu()
        {
            _exitToMainMenuRequest.OnNext(Unit.Default);
        }
    }
}
