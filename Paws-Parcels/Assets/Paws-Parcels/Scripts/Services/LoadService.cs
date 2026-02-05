using System;
using System.Globalization;
using System.Linq;
using ObservableCollections;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Services;
using UnityEngine;

namespace Paws_Parcels.Scripts
{
    public class LoadService
    {
        public Observable<Guid> LoadCoreSceneWithSaveId => _loadCoreSceneWithSaveIdSubject;
        public ReadOnlyReactiveProperty<CultureInfo> CurrentCultureInfo { get; }
        
        public IReadOnlyObservableDictionary<Guid, GameMeta> MetasMap { get; }
        
        private readonly Subject<Guid> _loadCoreSceneWithSaveIdSubject = new();
        private readonly GameStateProvider _gameStateProvider;
        private readonly LocalizationService _localizationService;

        public LoadService(GameStateProvider gameStateProvider, LocalizationService localizationService)
        {
            _gameStateProvider = gameStateProvider;
            _localizationService = localizationService;

            MetasMap = gameStateProvider.MetaMap;

            CurrentCultureInfo = localizationService.CurrentLocale.Select(x => x.Identifier.CultureInfo)
                .ToReadOnlyReactiveProperty();
        }

        public void LoadGame(Guid idSave)
        {
            _loadCoreSceneWithSaveIdSubject.OnNext(idSave);
        }

        public void DeleteSave(Guid idSave)
        {
            _gameStateProvider.DeleteGameState(idSave);
        }

        public void SaveGame()
        {
            _gameStateProvider.SaveGameState(false);
        }

        public void UpdateDataInFolderOfSave()
        {
            _gameStateProvider.UpdateSavesInSavesFolder();
        }

        public Texture2D GetTextureOrDefault(Guid idSave)
        {
            return _gameStateProvider.GetScreenshotOrDefault(idSave);
        }

        public string GetQuestWithLocalization(string questCode)
        {
            return _localizationService.GetCurrentLanguageStringOrDefault(questCode, Tables.Quests);
        }
        
        public string GetLocationWithLocalization(string locCode)
        {
            return _localizationService.GetCurrentLanguageStringOrDefault(locCode, Tables.Locations);
        }
    }
}