using System;
using R3;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace MotorHome.Scripts
{
    [Serializable]
    public class DialogReference
    {
        public DialogEnum Dialog;
        public bool IsNextDialogAfter;
    }
    
    public class NPCInteractable : InteractableObject
    {
        [SerializeField] private DialogShower _npcDialogShower; 
        [SerializeField] private DialogReference[] _dialogs;
        [SerializeField] private UnityEvent _endUnityEvent;
        [SerializeField] private UnityEvent _afterDialogExtraNextUnityEvent;
        
        private Dialog _dialog;
        private LanguageChanger _languageChanger;

        public ReactiveProperty<int> _currentReference = new(0);

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
            if(_dialogs.Length == 0) return;
            
            var dialog = _languageChanger.GetDialog(_dialogs[_currentReference.Value].Dialog);
            _dialog.StartDialog(dialog, _npcDialogShower).Subscribe(_ =>
            {
                print("dialog ended");
                End.OnNext(Unit.Default);
                _endUnityEvent.Invoke();

                if (_dialogs[_currentReference.Value].IsNextDialogAfter)
                {
                    _currentReference.Value++;
                    _afterDialogExtraNextUnityEvent.Invoke();
                }
                //задание дать
            });
        }

        public void NextDialog()
        {
            _currentReference.Value++;
        }
    }
}