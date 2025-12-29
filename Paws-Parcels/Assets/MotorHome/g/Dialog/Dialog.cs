using R3;
using Zenject;

namespace MotorHome.Scripts
{
    public class Dialog
    {
        public bool IsStarted { private set; get; } = false;

        private LanguageChanger _languageChanger;
        private IDialogShower _playerDialogShower;
        private ComboDialog _currentComboDialog = null;
        private IStopMove _stopMove;

        public Dialog(LanguageChanger languageChanger, SoloMonolog soloMonolog, IStopMove stopMove)
        {
            _languageChanger = languageChanger;
            _playerDialogShower = soloMonolog.DialogShower;
            _stopMove = stopMove;
        }

        public Observable<Unit> StartDialog(PhrasesOneManData[] dialog, DialogShower npcDialogShower)
        {
            if (_currentComboDialog != null)
            {
                _currentComboDialog.Dispose();
            }
            _currentComboDialog = new ComboDialog(npcDialogShower, _playerDialogShower);
            var observable = _currentComboDialog.StartDialog(dialog);
            var currentComboDialog = _currentComboDialog;
            observable.Take(1).Subscribe(_ =>
            {
                IsStarted = false;
                _stopMove.StartMove();
                currentComboDialog.Dispose();
                _currentComboDialog = null;
            });
            
            _stopMove.StopMove();
            IsStarted = true;
            
            return observable;
        }

        public void Click()
        {
            _currentComboDialog.Click();
        }
    }
}