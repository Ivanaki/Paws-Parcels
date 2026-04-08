using R3;
using UI.Core;
using UI.MVVM;

namespace UI.Screens.Loose
{
    public class ScreenLooseViewModel : WindowViewModel
    {
        private readonly CoreUIManager _coreUIManager;
        private readonly Subject<Unit> _exitToMainMenuRequest;
        private readonly Subject<Unit> _newGameRequest;

        public override string Id => "Loose";
        public override bool CanClose => false;

        public ScreenLooseViewModel(CoreUIManager coreUIManager, Subject<Unit> exitToMainMenuRequest,
            Subject<Unit> newGameRequest)
        {
            _coreUIManager = coreUIManager;
            _exitToMainMenuRequest = exitToMainMenuRequest;
            _newGameRequest = newGameRequest;
        }

        public void RequestOpenLoadMenu()
        {
            _coreUIManager.OpenPopupLoadMenu(true);
        }

        public void RequestNewGame()
        {
            _newGameRequest.OnNext(Unit.Default);
        }

        public void RequestGoToMainMenu()
        {
            _exitToMainMenuRequest.OnNext(Unit.Default);
        }
    }
}
