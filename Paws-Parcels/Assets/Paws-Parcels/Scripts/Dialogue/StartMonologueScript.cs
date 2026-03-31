using Paws_Parcels.NPCDialogs;
using UnityEngine;
using Yarn.Unity;
using Zenject;

namespace Dialogue
{
    public class StartMonologueScript : MonoBehaviour
    {
        [Inject] private DialogueService _dialogueService;

        [SerializeField] private DialogueReference _dialogueReference;

        public void StartMonologue(DialogueReference dialogueReference)
        {
            _dialogueService.TryStartMonologue(dialogueReference);
        }

        public void StartMonologue()
        {
            _dialogueService.TryStartMonologue(_dialogueReference);
        }
    }
}