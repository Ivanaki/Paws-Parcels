using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ObservableCollections;
using Paws_Parcels.Scripts.Quests;
using R3;
using SaveLaod;
using Saves;
using Services;
using UnityEngine;

namespace Paws_Parcels.Scripts.Saves.States.GameState
{
    public class GameStateProvider
    {
        public class GameStateProviderLoadCallback
        {
            public bool IsSuccess;
            public string MessageToUser;
            public Exception Exception;
        }
        
        private const int GameVersion = 1;
        private const int SaveVersion = 1;
        private const string GameSaveFolder = "Saves";
        private const string HandSaveName = "HandSave";
        private const string AutoSaveName = "AutoSave";
        private const string GameStateName = "GameState";
        private const string MetaName = "Meta";
        private const string ScreenshotName = "Screenshot";
        
        public GameProxy GameProxy { get; }
        public IReadOnlyObservableDictionary<Guid, GameMeta> MetaMap => _metaMap;

        /// <summary>
        /// For rigidbody saves
        /// </summary>
        public Observable<Unit> CollectDataBeforeSave => _collectDataBeforeSave;
        /// <summary>
        /// For rigidbody load
        /// </summary>
        public Observable<Unit> ApplyDataAfterLoad => _applyDataAfterLoad;
        
        private readonly Subject<Unit> _collectDataBeforeSave = new();
        private readonly Subject<Unit> _applyDataAfterLoad = new();

        private readonly JsonFileSerializer _jsonSerializer;
        private readonly ScreenshotSaver _screensaver;
        //private readonly QuestsService _questsService;
        private readonly PlayerLocationService _playerLocationService;

        private readonly GameState _gameStateOrigin;
        
        private readonly ObservableDictionary<Guid, GameMeta> _metaMap = new();

        private int AutoSaveNewNumber;
        private int HandSaveNewNumber;

        public GameStateProvider(JsonFileSerializer jsonSerializer, ScreenshotSaver screensaver,
            /*QuestsService questsService, */PlayerLocationService playerLocationService)
        {
            _jsonSerializer = jsonSerializer;
            _screensaver = screensaver;
            //_questsService = questsService;
            _playerLocationService = playerLocationService;

            _gameStateOrigin = new GameState();
            GameProxy = new GameProxy(_gameStateOrigin);
            
            UpdateSavesInSavesFolder();
        }

        public void UpdateSavesInSavesFolder()
        {
            _metaMap.Clear();
            
            var regexAutoSave = new Regex($"^{AutoSaveName}(\\d+)$");
            var regexHandSave = new Regex($"^{HandSaveName}(\\d+)$");

            var maximumAutoSaveNumber = -1;
            var maximumHandSaveNumber = -1;
            
            //сделать словарь с meta файлами по гуиду
            var allFolders = SaveStatic.GetFoldersNameInFolder(GameSaveFolder);
            foreach (var folder in allFolders)
            {
                var metaFileCallback = _jsonSerializer.Load<GameMeta>(MetaName, GameSaveFolder, folder);
                if (metaFileCallback.Callback == FileOperationResult.Success)
                {
                    var meta = metaFileCallback.Value;
                    
                    //хочу чтобы папка была самим названием сохранения
                    meta.SaveName = folder;
                    
                    //добавляем элемент
                    if (_metaMap.TryAdd(meta.SaveId, meta))
                    {
                        var autoMatch = regexAutoSave.Match(meta.SaveName);
                        if (autoMatch.Success)
                        {
                            if (int.TryParse(autoMatch.Groups[1].Value, out var value))
                            {
                                if (value > maximumAutoSaveNumber) 
                                {
                                    maximumAutoSaveNumber = value;
                                }
                            }
                        }

                        var handMatch = regexHandSave.Match(meta.SaveName);
                        if (handMatch.Success)
                        {
                            if (int.TryParse(handMatch.Groups[1].Value, out var value))
                            {
                                if (value > maximumHandSaveNumber)
                                {
                                    maximumHandSaveNumber = value;
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("meta file is duplicated");
                    }
                }
                else if(metaFileCallback.Callback == FileOperationResult.NotFound)
                {
                    Debug.LogError("meta file not found in directory: " + GameSaveFolder + "/" + folder);
                }
                else
                {
                    Debug.LogError(metaFileCallback.Exception?.Message);
                }
            }
            
            AutoSaveNewNumber = maximumAutoSaveNumber + 1;
            HandSaveNewNumber = maximumHandSaveNumber + 1;
        }
        
        public GameStateProviderLoadCallback LoadGameState(Guid id)
        {
            if (_metaMap.TryGetValue(id, out var meta))
            {
                var callBack = _jsonSerializer.Load<GameState>(GameStateName, GameSaveFolder, meta.SaveName);

                if (callBack.Callback == FileOperationResult.Success)
                {
                    var state = callBack.Value;
                    GameProxy.Change(state);
                    Debug.Log($"Game state loaded: {meta.SaveName}");
                    _applyDataAfterLoad.OnNext(Unit.Default);
                    return new GameStateProviderLoadCallback(){IsSuccess = true};
                }
                
                if (callBack.Callback == FileOperationResult.NotFound)
                {
                    return new GameStateProviderLoadCallback()
                        { IsSuccess = false, Exception = callBack.Exception, MessageToUser = "Сохранение утеряно" };
                }
                
                return new GameStateProviderLoadCallback(){IsSuccess = false, Exception = callBack.Exception, MessageToUser = "Load error"};
            }
            
            var exp = new Exception("mistake programmer, how you get this meta in method UpdateSavesInSavesFolder?");
            Debug.LogError(exp.Message);
            return new GameStateProviderLoadCallback(){IsSuccess = false, Exception = exp, MessageToUser = "Load error"};
        }

        public bool SaveGameState(bool isAutoSave)
        {
            _collectDataBeforeSave.OnNext(Unit.Default);
            
            var newID = Guid.NewGuid();
            
            var dateTime = DateTime.UtcNow;
            
            var saveName = isAutoSave ? AutoSaveName + AutoSaveNewNumber++ : HandSaveName + HandSaveNewNumber++;
            
            _gameStateOrigin.SaveId = newID;
            
            var meta = new GameMeta()
            {
                SaveId = newID,
                SaveName = saveName,
                SaveDataTimeUtc = dateTime,
                TrackedQuestCode = /*_questsService.GetQuestCode(GameProxy.TrackedMissionID.Value)*/"dddddddd",
                CurrentLocationCode = _playerLocationService.GetCurrentLocationCode(),
                IsAutoSave = isAutoSave,
                GameVersion = GameVersion,
                SaveVersion = SaveVersion
            };

            var folders = new string[] {GameSaveFolder, saveName};
            
            var callbackGameState = _jsonSerializer.Save(_gameStateOrigin, GameStateName, folders);
            var callbackMetaData = _jsonSerializer.Save(meta, MetaName, folders);
            var callbackImage = _screensaver.CaptureScreenshot(ScreenshotName, Camera.main, folders);

            if (!callbackImage.IsComplete)
            {
                Debug.LogError(callbackImage.Exception?.Message);
            }
            
            if (callbackGameState.IsComplete && callbackMetaData.IsComplete)
            {
                _metaMap.Add(newID, meta);
                //_saveResultSubject.OnNext(true);
                return true;
            }

            _jsonSerializer.Delete(GameStateName, folders);
            _jsonSerializer.Delete( MetaName, folders);
            _screensaver.DeleteScreenshot(ScreenshotName, folders);
            
            Debug.LogError(callbackGameState.Exception?.Message);
            Debug.LogError(callbackMetaData.Exception?.Message);
            //_saveResultSubject.OnNext(false);
            return false;
        }

        public bool DeleteGameState(Guid id)
        {
            if (_metaMap.TryGetValue(id, out var meta))
            {
                var result = _jsonSerializer.DeleteFolder(GameSaveFolder, meta.SaveName);

                if (result)
                {
                    _metaMap.Remove(id);
                }
                
                return result;
            }
            
            Debug.LogError("mistake programmer, how you get this meta in method UpdateSavesInSavesFolder?");
            return false;
        }
        
        public bool LoadNewGame()
        {
            try
            {
                var baseGameSo = Resources.Load<BaseGameSO>("BaseStates/BaseGameSO");
                GameProxy.Change(baseGameSo.GameState);
                _applyDataAfterLoad.OnNext(Unit.Default);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        public GameStateProviderLoadCallback LoadLastSave()
        {
            var metas = MetaMap.Values.ToList();
            if (metas.Count == 0)
            {
                Debug.LogWarning("Last save not found => Loading new game");
                var result = LoadNewGame();
                
                if (result) return new GameStateProviderLoadCallback() { IsSuccess = true };
                
                return new GameStateProviderLoadCallback()
                {
                    IsSuccess = false,
                    Exception = new Exception("Error already throw"),
                    MessageToUser = "Error while loading new game while loading last save"
                };
            }
            var eatherMeta = metas.OrderByDescending(x => x.SaveDataTimeUtc).First();
            return LoadGameState(eatherMeta.SaveId);
        }

        public Texture2D GetScreenshotOrDefault(Guid id)
        {
            if (_metaMap.TryGetValue(id, out var meta))
            {
                var folders = new string[] {GameSaveFolder, meta.SaveName};
                var callback = _screensaver.LoadTexture(ScreenshotName, folders);

                if (callback.Callback == FileOperationResult.Success)
                {
                    return callback.Texture;
                }

                if (callback.Callback == FileOperationResult.NotFound)
                {
                    Debug.LogError("Image not found in folder");
                    return null;
                }

                if (callback.Callback == FileOperationResult.Error)
                {
                    Debug.LogError(callback.Exception.Message);
                    return null;
                }
            }
            
            Debug.LogError("SaveId not found");
            return null;
        }
    }
}