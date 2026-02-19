using System;
using System.Collections.Generic;
using MotorHome.Scripts.Saves.Core;
using MotorHome.Scripts.Saves.ScriptableObjects;
using R3;
using SaveLaod;
using UnityEngine;

namespace MotorHome.Scripts.Saves
{
    public class GameStateProvider
    {
        private readonly ILoadSaveService _loadSaveService;
        private readonly ScreenshotSaver _screenshotSaver;
        private readonly SettingsState _baseSettingsState;
        private readonly CoreState _baseCoreState;

        private SettingsState _settingsStateOrigin;
        private CoreState _coreStateOrigin;

        private const string GAME_SETTINGS_STATE_KEY = nameof(GAME_SETTINGS_STATE_KEY);
        private const string GAME_CORE_STATE_KEY = nameof(GAME_CORE_STATE_KEY);
        
        public GameStateProvider(ILoadSaveService loadSaveService, SettingsSo baseSettingsStateSo,
            CoreSo baseCoreStateSo, ScreenshotSaver screenshotSaver)
        {
            _loadSaveService = loadSaveService;
            _screenshotSaver = screenshotSaver;
            _baseSettingsState = baseSettingsStateSo.SettingsState;
            _baseCoreState = baseCoreStateSo.CoreState;
        }

        public CoreState CoreState { get; private set; }
        public ReadOnlyReactiveProperty<SettingsStateProxy> SettingsStateProxy => _settingsStateProxy;
        
        public Observable<Unit> Updated => _coreStateProxyUpdated;
        public Observable<Unit> Saved => _coreStateProxySaved;
        
        private readonly ReactiveProperty<SettingsStateProxy> _settingsStateProxy = new(null);
        private readonly Subject<Unit> _coreStateProxyUpdated = new();
        private readonly Subject<Unit> _coreStateProxySaved = new();
        
        public Observable<SettingsStateProxy> LoadSettingsState()
        {
            _loadSaveService.Load<SettingsState>(GAME_SETTINGS_STATE_KEY).Subscribe(callback =>
            {
                switch (callback.Callback)
                {
                    case SavesStateEnum.FileWithKeyDontExists:
                        
                        _settingsStateOrigin = CreateSettingsStateFromBaseSettings();
                        _settingsStateProxy.Value = new SettingsStateProxy(_settingsStateOrigin);
                        SaveSettingsState();    // Сохраним дефолтное состояние
                        
                        break;
                    case SavesStateEnum.Complete:
                        
                        // Загружаем
                        _settingsStateOrigin = (SettingsState)callback.Value;
                        _settingsStateProxy.Value = new SettingsStateProxy(_settingsStateOrigin);
                        //Debug.Log(SettingsStateProxy.MasterVolume.CurrentValue);
                        
                        break;
                    case SavesStateEnum.Error:
                        throw new Exception("Error");
                }
            });
            
            return Observable.Return(SettingsStateProxy.CurrentValue);
        }
        
        public Observable<bool> SaveSettingsState()
        {            
            var returnObservable = _loadSaveService.Save(GAME_SETTINGS_STATE_KEY, _settingsStateOrigin);

            return returnObservable;
        }
        
        public Observable<SettingsStateProxy> ResetSettingsState()
        {
            _settingsStateOrigin = CreateSettingsStateFromBaseSettings();
            _settingsStateProxy.Value = new SettingsStateProxy(_settingsStateOrigin);
            SaveSettingsState();
            
            return Observable.Return(SettingsStateProxy.CurrentValue);
        }
        
        private SettingsState CreateSettingsStateFromBaseSettings()
        {
            // Состояние по умолчанию из настроек
            return new SettingsState(_baseSettingsState);
        }
        
        
        
        
        
        
        
        
        // //////////////////////////////////////////////////////////////////////////

        public void DeleteCoreState(int saveNumber)
        {
            _screenshotSaver.DeleteScreenshot(saveNumber.ToString());
            _loadSaveService.Delete(saveNumber.ToString());
        }
        
        public Observable<CoreState> LoadCoreState(int number)
        {
            _loadSaveService.Load<CoreState>(number.ToString()).Subscribe(callback =>
            {
                switch (callback.Callback)
                {
                    case SavesStateEnum.FileWithKeyDontExists:
                        
                        _coreStateOrigin = CreateCoreStateFromBaseCoreData();
                        //CoreStateProxy = new CoreStateProxy(_coreStateOrigin);
                        //Debug.Log("LoadCoreState " + CoreState.Items.Length);
                        CoreState = _coreStateOrigin;
                        //Debug.Log("LoadCoreState " + CoreState.Items.Length);
                        
                        break;
                    case SavesStateEnum.Complete:
                        
                        _coreStateOrigin = (CoreState)callback.Value;
                        //CoreStateProxy = new CoreStateProxy(_coreStateOrigin);
                        CoreState = _coreStateOrigin;
                        
                        break;
                    case SavesStateEnum.Error:
                        throw new Exception("Error");
                }
                
                //Debug.Log("LoadCoreState " + CoreState.Items.Length);
                _coreStateProxyUpdated.OnNext(Unit.Default);
            });
            
            return Observable.Return(CoreState);
        }
        
        public Observable<bool> SaveCoreState(string saveNumber)
        {        
            _coreStateProxySaved.OnNext(Unit.Default);
            var returnObservable = _loadSaveService.Save(saveNumber, _coreStateOrigin);

            return returnObservable;
        }

        public void CaptureScreenshot(string screenshotName)
        {
            _screenshotSaver.CaptureScreenshot(screenshotName, GetCamera());
        }

        public ScreenShotCallback LoadScreenshot(string screenshotName)
        {
            //return _screenshotSaver.LoadTexture(screenshotName);
            return null;
        }

        public Observable<bool> SaveCoreSaverData(BaseCoreSaverData baseCoreSaverData)
        {
            var returnObservable = _loadSaveService.Save(GAME_CORE_STATE_KEY, baseCoreSaverData);

            return returnObservable;
        }
        
        public BaseCoreSaverData LoadCoreSaverData()
        {
            BaseCoreSaverData subject = null;
            _loadSaveService.Load<BaseCoreSaverData>(GAME_CORE_STATE_KEY).Subscribe(callback =>
            {
                switch (callback.Callback)
                {
                    case SavesStateEnum.FileWithKeyDontExists:

                        subject = new BaseCoreSaverData(-1, new Dictionary<int, CoreSaveData>());
                        
                        break;
                    case SavesStateEnum.Complete:

                        subject = (BaseCoreSaverData)callback.Value;
                        
                        break;
                    case SavesStateEnum.Error:
                        throw new Exception("Error");
                }
            });

            return subject;
        }

        private Camera GetCamera()
        {
            return Camera.main;
        }
        
        private CoreState CreateCoreStateFromBaseCoreData()
        {
            // Состояние по умолчанию из настроек
            //Debug.Log("Base" + _baseCoreState.Items.Length);
            return new CoreState(_baseCoreState);
        }
    }
}