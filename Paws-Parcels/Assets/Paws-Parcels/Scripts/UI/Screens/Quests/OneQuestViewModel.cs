using System;
using R3;
using Services;

namespace UI.Screens.Quests
{
    public class OneQuestViewModel : IDisposable
    {
        public string QuestId { get; }
        public bool IsCompleted { get; }

        public ReadOnlyReactiveProperty<bool> IsSelected => _isSelected;
        public ReadOnlyReactiveProperty<bool> IsSelectedIdData => _isSelectedInData;

        public ReadOnlyReactiveProperty<string> TextToShow => _textToShow;

        private readonly ScreenQuestsViewModel _viewModelMain;
        private readonly string _questTitleCode;
        private readonly ReactiveProperty<bool> _isSelected = new(false);
        private readonly ReactiveProperty<bool> _isSelectedInData = new(false);
        private readonly ReactiveProperty<string> _textToShow = new("test");
        
        private CompositeDisposable _disposables = new();

        public OneQuestViewModel(ScreenQuestsViewModel viewModelMain, string questId, bool isCompleted)
        {
            _viewModelMain = viewModelMain;
            
            QuestId = questId;
            IsCompleted = isCompleted;

            viewModelMain.SelectedQuestInData.Subscribe(v => _isSelectedInData.Value = v == QuestId);
            
            viewModelMain.LocalizationService.CurrentLocale.Subscribe(locale =>
            {
                _textToShow.Value =
                    viewModelMain.LocalizationService.GetCurrentLanguageStringOrDefault(QuestId, Tables.Quests);
            });
        }

        public void RequestClicked()
        {
            _viewModelMain.RequestOneSaveClicked(QuestId);
        }

        public void Select()
        {
            _isSelected.Value = true;
        }

        public void Unselect()
        {
            _isSelected.Value = false;
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}