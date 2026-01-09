using System;
using Game.Root;
using Newtonsoft.Json;
using R3;
using SaveLaod;
using Saves;
using UnityEngine;

namespace Paws_Parcels.Scripts.Saves.States
{
    public class SettingsStateProvider
    {
        public GameSettings Settings { get; } 
        public Observable<(FileOperationResult, Exception)> LoadError => _loadErrorSubject;
        public Observable<bool> SaveResult => _saveResultSubject;

        private const string GameSettingsKey = "Base";
        private const string GameSettingsFolder = "Settings";

        private readonly JsonFileSerializer _jsonSerializer;

        private readonly Subject<(FileOperationResult, Exception)> _loadErrorSubject = new();
        private readonly Subject<bool> _saveResultSubject = new();

        private readonly GameSettingsState _gameSettingsStateOrigin;

        public SettingsStateProvider(JsonFileSerializer jsonSerializer, ScreenshotSaver screensaver)
        {
            _jsonSerializer = jsonSerializer;

            _gameSettingsStateOrigin = new GameSettingsState();
            Settings = new GameSettings(_gameSettingsStateOrigin);
        }

        public bool LoadSettingsState()
        {
            var callBack = _jsonSerializer.Load<GameSettingsState>(GameSettingsKey, GameSettingsFolder);

            if (callBack.Callback == FileOperationResult.Success)
            {
                var state = callBack.Value;
                Settings.Change(state);
                return true;
            }

            _loadErrorSubject.OnNext((callBack.Callback, callBack.Exception));
            return false;
        }

        public bool SaveSettingsState()
        {
            var callBack = _jsonSerializer.Save(_gameSettingsStateOrigin, GameSettingsKey, GameSettingsFolder);

            _saveResultSubject.OnNext(callBack.IsComplete);
            return callBack.IsComplete;
        }

        public void ResetSettingsState()
        {
            CreateGameSettingsFromSettings();
            SaveSettingsState();
        }

        private void CreateGameSettingsFromSettings()
        {
            var baseSettingsSo = Resources.Load<BaseSettingsSO>("BaseStates/BaseSettingsSO");
            Settings.Change(baseSettingsSo.BaseSettings);
        }
    }
}