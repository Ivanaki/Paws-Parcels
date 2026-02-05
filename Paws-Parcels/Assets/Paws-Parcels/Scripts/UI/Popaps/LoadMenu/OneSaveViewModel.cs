using System;
using System.Globalization;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;

namespace UI.Screens.LoadMenu
{
    public class OneSaveViewModel
    {
        public DateTime DateTimeUtc { get; }
        public Guid Id { get; }
        public ReadOnlyReactiveProperty<bool> IsSelected => _isSelected;
        //public string TextToShow { get; }
        public ReadOnlyReactiveProperty<string> TextToShow => _textToShow;
        
        //private readonly Guid _id;
        private readonly PopupLoadMenuViewModel _viewModelMain;
        private readonly ReactiveProperty<bool> _isSelected = new(false);
        private readonly ReactiveProperty<string> _textToShow = new("test");

        public OneSaveViewModel(GameMeta gameMeta, PopupLoadMenuViewModel viewModelMain)
        {
            //_id = gameMeta.SaveId;
            _viewModelMain = viewModelMain;
            Id = gameMeta.SaveId;
            DateTimeUtc = gameMeta.SaveDataTimeUtc;
            

            viewModelMain.CurrentCultureInfo.Subscribe(culture =>
            {
                _textToShow.Value = DateTimeUtc.ToLocalTime().ToString("F", culture)/* + " - " + gameMeta.SaveName*/;
            });
        }

        public void RequestClicked()
        {
            _viewModelMain.RequestOneSaveClicked(Id);
        }

        public void Select()
        {
            _isSelected.Value = true;
        }
        public void Unselect()
        {
            _isSelected.Value = false;
        }
    }
}