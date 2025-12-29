using R3;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class NPCInteractable : InteractableObject
    {
        [SerializeField] private DialogShower _npcDialogShower; 
        [SerializeField] private DialogEnum _dialogEnum;
        
        private Dialog _dialog;
        private LanguageChanger _languageChanger;

        public Subject<Unit> End = new();
        
        [Inject]
        private void Construct(Dialog dialog, LanguageChanger languageChanger)
        {
            _dialog = dialog;
            _languageChanger = languageChanger;
        }
        
        public override void Interact()
        {
            print("interacted");
            
            var dialog = _languageChanger.GetDialog(_dialogEnum);
            _dialog.StartDialog(dialog, _npcDialogShower).Subscribe(_ =>
            {
                print("dialog ended");
                End.OnNext(Unit.Default);
                //задание дать
            });
        }
    }
}