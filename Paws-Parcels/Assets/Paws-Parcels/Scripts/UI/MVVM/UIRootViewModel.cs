using System;
using System.Collections.Generic;
using System.Linq;
using ObservableCollections;
using R3;
using Services;
using UI.DialogueScreen;

namespace UI.MVVM
{
    public class UIRootViewModel : IDisposable
    {
        public ReadOnlyReactiveProperty<WindowViewModel> OpenedScreen => _openedScreen;
        public IObservableCollection<WindowViewModel> OpenedPopups => _openedPopups;

        private readonly ReactiveProperty<WindowViewModel> _openedScreen = new(null);
        private readonly ObservableList<WindowViewModel> _openedPopups = new();
        private readonly Dictionary<WindowViewModel, IDisposable> _popupSubscriptions = new();
        
        public void Dispose()
        {
            CloseAllPopups();
            _openedScreen.Value?.Dispose();
        }

        public void OpenScreen(WindowViewModel screenViewModel)
        {
            _openedScreen.Value?.Dispose();
            _openedScreen.Value = screenViewModel;
        }

        public bool TryCloseScreen()
        {
            if (_openedScreen.Value is ScreenDialogueViewModel)
                return false;
            
            _openedScreen.Value?.Dispose();
            _openedScreen.Value = null;
            return true;
        }

        public void OpenPopup(WindowViewModel popupViewModel)
        {
            if (_openedPopups.Contains(popupViewModel))
            {
                return;
            }

            var subscription = popupViewModel.CloseRequested.Subscribe(ClosePopup);
            _popupSubscriptions.Add(popupViewModel, subscription);
            _openedPopups.Add(popupViewModel);
        }

        public void ClosePopup(WindowViewModel popupViewModel)
        {
            if (_openedPopups.Contains(popupViewModel))
            {
                popupViewModel.Dispose();
                _openedPopups.Remove(popupViewModel);

                var popupSubscription = _popupSubscriptions[popupViewModel];
                popupSubscription?.Dispose();
                _popupSubscriptions.Remove(popupViewModel);
            }
        }

        public void ClosePopup(string popupId)
        {
            var openedPopupViewModel = _openedPopups.FirstOrDefault(p => p.Id == popupId);
            ClosePopup(openedPopupViewModel);
        }

        public void CloseAllPopups()
        {
            foreach (var openedPopup in _openedPopups)
            {
                ClosePopup(openedPopup);
            }
        }
        
        public bool TryCloseLastPopup()
        {
            if (_openedPopups.Count > 0)
            {
                var lastPopupViewModel = _openedPopups.Last();
                
                ClosePopup(lastPopupViewModel);
                return true;
            }
            return false;
        }
    }
}