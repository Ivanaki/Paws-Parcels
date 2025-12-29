using R3;

namespace Game.Root
{
    public class MainMenuScenesChanger
    {
        public Observable<Unit> CoreSignal => _coreSignalSub;
        public Observable<Unit> RestartSignal => _restartSignalSub;
        public Observable<Unit> ExitToDesktopSignal => _exitToDesktopSignalSub;
        
        private readonly Subject<Unit> _restartSignalSub = new();
        private readonly Subject<Unit> _coreSignalSub = new();
        private readonly Subject<Unit> _exitToDesktopSignalSub = new();
        
        public void GoToCore()
        {
            _coreSignalSub.OnNext(Unit.Default);
        }

        public void Restart()
        {
            _restartSignalSub.OnNext(Unit.Default);
        }
        
        public void Exit()
        {
            _exitToDesktopSignalSub.OnNext(Unit.Default);
        }
    }
}