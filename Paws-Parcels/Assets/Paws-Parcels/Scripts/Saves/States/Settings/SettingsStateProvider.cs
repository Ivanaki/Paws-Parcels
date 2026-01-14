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
        private const string GameSettingsKey = "Base";
        private const string GameSettingsFolder = "Settings";
        
        public SettingsProxy SettingsProxy { get; } 
        public Observable<(FileOperationResult, Exception)> LoadError => _loadErrorSubject;
        public Observable<bool> SaveResult => _saveResultSubject;

        private readonly JsonFileSerializer _jsonSerializer;

        private readonly Subject<(FileOperationResult, Exception)> _loadErrorSubject = new();
        private readonly Subject<bool> _saveResultSubject = new();

        private readonly SettingsState _settingsStateOrigin;

        public SettingsStateProvider(JsonFileSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;

            _settingsStateOrigin = new SettingsState();
            SettingsProxy = new SettingsProxy(_settingsStateOrigin);
        }

        public bool LoadSettingsState()
        {
            var callBack = _jsonSerializer.Load<SettingsState>(GameSettingsKey, GameSettingsFolder);

            if (callBack.Callback == FileOperationResult.Success)
            {
                var state = callBack.Value;
                SettingsProxy.Change(state);
                return true;
            }

            _loadErrorSubject.OnNext((callBack.Callback, callBack.Exception));
            return false;
        }

        public bool SaveSettingsState()
        {
            var callBack = _jsonSerializer.Save(_settingsStateOrigin, GameSettingsKey, GameSettingsFolder);

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
            SettingsProxy.Change(baseSettingsSo.BaseSettings);
        }
    }
}