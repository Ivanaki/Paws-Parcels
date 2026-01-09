using R3;

namespace Game.Root
{
    public class MainMenuSignals
    {
        public Observable<Unit> RestartSignal => _restartSignalSubject;
        public Observable<Unit> NewGameSignal => _newGameSignalSubject;
        public Observable<Unit> ExitToDesktopSignal => _exitToDesktopSignalSubject;
        
        private readonly Subject<Unit> _restartSignalSubject = new();
        private readonly Subject<Unit> _newGameSignalSubject = new();
        private readonly Subject<Unit> _exitToDesktopSignalSubject = new();

        public void Restart()
        {
            _restartSignalSubject.OnNext(Unit.Default);
        }

        public void StartNewGame()
        {
            _newGameSignalSubject.OnNext(Unit.Default);
        }

        public void Quit()
        {
            _exitToDesktopSignalSubject.OnNext(Unit.Default);
        }
    }
}