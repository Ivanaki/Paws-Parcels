using R3;

namespace Services
{
    public class ShowableInterfaceService
    {
        public Observable<Unit> CloseAllObservable => _closeAllSubject;
        public Observable<Unit> BanOpenObservable => _banOpenSubject;
        public Observable<Unit> UnbanOpenObservable => _unbanOpenSubject;
        public ReadOnlyReactiveProperty<bool> HudActive => _hudActive;
        
        private readonly Subject<Unit> _closeAllSubject = new();
        private readonly Subject<Unit> _banOpenSubject = new();
        private readonly Subject<Unit> _unbanOpenSubject = new();
        private readonly ReactiveProperty<bool> _hudActive = new(true);
        
        public void CloseAllInterface()
        {
            _closeAllSubject.OnNext(Unit.Default);
        }

        public void HideHud()
        {
            _hudActive.Value = false;
        }
        
        public void ShowHud()
        {
            _hudActive.Value = true;
        }
        
        public void BanOpen()
        {
            _banOpenSubject.OnNext(Unit.Default);
        }

        public void UnbanOpen()
        {
            _unbanOpenSubject.OnNext(Unit.Default);
        }
    }
}