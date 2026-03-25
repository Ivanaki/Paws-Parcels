using System;
using NaughtyAttributes;
using UI.DialogueScreen;
using UI.MVVM;
using UnityEngine;
using Yarn.Unity;

namespace NPCDialogs
{
    public class ScreenDialogueBinder : WindowBinder<ScreenDialogueViewModel>
    {
        [SerializeField] private DialogueRunner _dialogueRunner;
        
        protected override void OnBind(ScreenDialogueViewModel viewModel)
        {
            ViewModel.SetRunner(_dialogueRunner);
        }
        
        private void OnDestroy()
        {
            ViewModel?.SetRunner(null);
        }
    }
}