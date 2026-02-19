using System;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class MessageGame : IInitializable, IDisposable
    {
        private readonly MessageGameUI _messageGameUI;
        
        public MessageGame(MessageGameUI messageGameUI)
        {
            _messageGameUI = messageGameUI;
        }
        
        public void Initialize()
        {
            
        }

        public void DoMessage(string message, float duration = 1.5f)
        {
            _messageGameUI.Send(message, duration);
        }
        
        public void Dispose()
        {
            
        }
    }
}