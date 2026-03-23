using UI.MVVM;

namespace UI.DialogueScreen
{
    public class ScreenDialogueViewModel : WindowViewModel
    {
        /*private readonly CoreUIManager _coreUIManager;
        private readonly Subject<Unit> _exitToMainMenuRequest;
        private readonly LoadService _loadService;
        public override string Id => "Dialogue";

        public ScreenDialogueViewModel(CoreUIManager coreUIManager, Subject<Unit> exitToMainMenuRequest, LoadService loadService)
        {
            _coreUIManager = coreUIManager;
            _exitToMainMenuRequest = exitToMainMenuRequest;
            _loadService = loadService;
        }

        public void RequestBackToGame()
        {
            _coreUIManager.CloseScreen();
        }

        public void RequestSave()
        {
            _loadService.SaveGame();
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
        }*/
        public override string Id { get; }
    }
}