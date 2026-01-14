using System;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;

namespace UI.Screens.LoadMenu
{
    public class OneSaveViewModel
    {
        public DateTime DateTime { get; }
        public Guid Id { get; }
        public ReadOnlyReactiveProperty<bool> IsSelected => _isSelected;
        public ReadOnlyReactiveProperty<string> DateTimeText => _dateTimeText;
        
        private readonly Guid _id;
        private readonly PopupLoadMenuViewModel _viewModelMain;
        private readonly ReactiveProperty<bool> _isSelected = new(false);
        private readonly ReactiveProperty<string> _dateTimeText = new("test");

        public OneSaveViewModel(Guid id, DateTime dateTime, PopupLoadMenuViewModel viewModelMain)
        {
            Id = id;
            _id = id;
            DateTime = dateTime;
            _viewModelMain = viewModelMain;
            //viewModelMain.
        }

        public void RequestClicked()
        {
            _viewModelMain.RequestOneSaveClicked(_id);
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