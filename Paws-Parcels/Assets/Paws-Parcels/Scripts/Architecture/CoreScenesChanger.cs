using R3;

namespace Game.Root
{
    public class CoreScenesChanger
    {
        public Observable<Unit> MainMenuSignal => _mainMenuSignalSub;
        public Observable<Unit> RestartSignal => _restartSignalSub;
        
        private Subject<Unit> _restartSignalSub = new();
        private Subject<Unit> _mainMenuSignalSub = new();
        
        public void GoToMenu()
        {
            _mainMenuSignalSub.OnNext(Unit.Default);
        }

        public void Restart()
        {
            _restartSignalSub.OnNext(Unit.Default);
        }
    }
}