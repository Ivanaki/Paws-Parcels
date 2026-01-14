using System.Collections;
using System.Collections.Generic;
using Game.Root;
using R3;
using UnityEngine;

namespace Services
{
    public class MessageToUserService
    {
        private const float BaseMessageTime = 4f;
        
        public ReadOnlyReactiveProperty<string> MessageToMainMenu => _messageToMainMenu;
        public ReadOnlyReactiveProperty<Color> MessageToMainMenuColor => _messageToMainMenuColor;

        private readonly ReactiveProperty<string> _messageToMainMenu =  new("");
        private readonly ReactiveProperty<Color> _messageToMainMenuColor = new(Color.white);
        private readonly Coroutines _coroutines;
        
        private readonly Queue<(string, Color, float)> _messagesQueue = new();
        private Coroutine _currentCoroutine;
        
        public MessageToUserService(Coroutines coroutines)
        {
            _coroutines = coroutines;
        }
        
        public void SendMessage(string message, Color highlightColor, float time = BaseMessageTime)
        {
            _messagesQueue.Enqueue((message, highlightColor, time));
            CheckMessage();
        }

        private void CheckMessage()
        {
            if (_currentCoroutine != null) return;
            _currentCoroutine = _coroutines.StartCoroutine(MessagesCoroutine());
        }

        private IEnumerator MessagesCoroutine()
        {
            while (_messagesQueue.Count > 0)
            {
                var value = _messagesQueue.Dequeue();
                if (value.Item1 == "")
                    continue;
                _messageToMainMenu.Value = value.Item1;
                _messageToMainMenuColor.Value = value.Item2;
                yield return new WaitForSeconds(value.Item3);
            }

            _messageToMainMenu.Value = "";
            _currentCoroutine = null;
        }
    }
}