using Paws_Parcels.NPCDialogs;
using UI.MVVM;
using Yarn.Unity;

namespace UI.DialogueScreen
{
    public class ScreenDialogueViewModel : WindowViewModel
    {
        private readonly DialogueService _dialogueService;
        public override string Id => "Dialogue";

        public ScreenDialogueViewModel(DialogueService dialogueService)
        {
            _dialogueService = dialogueService;
        }
        
        public void SetRunner(DialogueRunner runner)
        {
            _dialogueService.SetRunner(runner);
        }
    }
}