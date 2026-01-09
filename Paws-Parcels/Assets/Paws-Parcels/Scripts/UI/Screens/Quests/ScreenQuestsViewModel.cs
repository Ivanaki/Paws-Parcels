using UI.Core;
using UI.MVVM;

namespace UI.Screens.Quests
{
    public class ScreenQuestsViewModel :  WindowViewModel
    {
        private readonly CoreUIManager _coreUIManager;
        public override string Id => "Quests";

        public ScreenQuestsViewModel(CoreUIManager coreUIManager)
        {
            _coreUIManager = coreUIManager;
        }
    }
}