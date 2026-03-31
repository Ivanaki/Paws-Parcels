using System.Globalization;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Services;

namespace Quests
{
    public class QuestsServiceForUi
    {
        private readonly GameStateProvider _gameStateProvider;
        private readonly GameProxy _gameProxy;
        
        public ReadOnlyReactiveProperty<CultureInfo> CurrentCultureInfo { get; }
        
        public QuestsServiceForUi(GameStateProvider gameStateProvider, LocalizationService localizationService)
        {
            _gameStateProvider = gameStateProvider;
            _gameProxy = _gameStateProvider.GameProxy;
            CurrentCultureInfo = localizationService.CurrentLocale.Select(x => x.Identifier.CultureInfo)
                .ToReadOnlyReactiveProperty();
        }

        public void SelectQuest(string questId)
        {
            _gameProxy.SelectedQuest.Value = questId;
        }
    }
}