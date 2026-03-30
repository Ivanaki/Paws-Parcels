using UI.Core;
using UI.MVVM;

namespace UI.Screens.Map
{
    public class ScreenMapViewModel : WindowViewModel
    {
        private readonly CoreUIManager _coreUIManager;
        
        public override string Id => "Map";

        public ScreenMapViewModel(CoreUIManager coreUIManager)
        {
            _coreUIManager = coreUIManager;
        }

        public void RequestBackToGame()
        {
            _coreUIManager.TryCloseScreen();
        }
    }
}