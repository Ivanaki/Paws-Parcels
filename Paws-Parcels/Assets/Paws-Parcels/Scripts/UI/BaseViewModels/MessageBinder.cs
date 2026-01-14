using R3;
using TMPro;
using UnityEngine;

namespace UI.HUD
{
    public class MessageBinder : MonoBehaviour
    {
        [SerializeField] private TMP_Text _messageText;
        
        private readonly CompositeDisposable _disposables = new();
        
        public void Bind(MessageViewModel viewModel)
        {
            _disposables.Add(viewModel.MessageToMainMenu.Subscribe(v => _messageText.text = v));
            _disposables.Add(viewModel.MessageToMainMenuColor.Subscribe(v => _messageText.color = v));
        }
        
        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}