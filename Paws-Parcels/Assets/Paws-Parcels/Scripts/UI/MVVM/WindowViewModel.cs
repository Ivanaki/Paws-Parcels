using System;
using R3;

namespace UI.MVVM
{
    public abstract class WindowViewModel : IDisposable
    {
        public Observable<WindowViewModel> CloseRequested => _closeRequested;
        public abstract string Id { get; }
        public virtual bool CanClose => true;

        private readonly Subject<WindowViewModel> _closeRequested = new();

        
        /// <summary>
        /// only for popups
        /// </summary>
        public void RequestClose()
        {
            if (CanClose == false)
            {
                return;
            }

            _closeRequested.OnNext(this);
        }

        public virtual void Dispose() { }
    }
}
