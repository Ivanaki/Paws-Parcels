using System;
using R3;

namespace Quests.Message
{
    public class ReactiveMessage : IMessageHandler, IDisposable
    {
        public Observable<MessageArgs> Observe => _observe;
        
        private Subject<MessageArgs> _observe = new();
        
        public void OnMessage(MessageArgs messageArgs)
        {
            _observe.OnNext(messageArgs);
        }

        public void Dispose()
        {
            _observe?.Dispose();
        }

        public void Clear()
        {
            Dispose();
            _observe = new Subject<MessageArgs>();
        }
    }
}