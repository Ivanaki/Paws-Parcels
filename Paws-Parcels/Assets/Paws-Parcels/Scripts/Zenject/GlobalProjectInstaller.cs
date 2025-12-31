using Game.Root;
using MotorHome.Scripts.Saves;
using MotorHome.Scripts.Saves.ScriptableObjects;
using MotorHome.Scripts.ScriptableObjects;
using SaveLaod;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace MotorHome.Scripts.Zenject
{
    public class GlobalProjectInstaller : MonoInstaller
    {
        [SerializeField] private UIRootView _uiRootPrefab;
        
        [SerializeField] private SettingsSo _baseSettingsSo;
        [SerializeField] private CoreSo _baseCoreSo;
        
        [SerializeField] private UIRootView _uiRootView;

        [SerializeField] private InputActionAsset _inputActionAsset;

        [SerializeField] private LanguageSO[] _languagesSO;
        [SerializeField] private GameSettingsSO _gameSettingsSo;
        [SerializeField] private QuestsSO _questsSo;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameEntryPoint>().AsSingle().NonLazy();
            Container.BindExecutionOrder<GameEntryPoint>(-1000);

            Container.Bind<UIRootView>().FromComponentInNewPrefab(_uiRootPrefab).AsSingle()
                .OnInstantiated<UIRootView>((ctx, ui) =>
                {
                    ui.ShowLoadingScreen();
                    ui.transform.SetParent(null);
                    DontDestroyOnLoad(ui.gameObject);
                }).NonLazy();
            
            Container.Bind<Coroutines>().FromNewComponentOnNewGameObject().WithGameObjectName("[COROUTINES]").AsSingle()
                .OnInstantiated<Coroutines>((ctx, c) =>
                {
                    c.transform.SetParent(null);
                    DontDestroyOnLoad(c.gameObject);
                }).NonLazy();
            
            
            
            
            Container.Bind<ScreenshotSaver>().AsSingle();
            
            Container.Bind<SettingsSo>().FromInstance(_baseSettingsSo).AsSingle().NonLazy();
            Container.Bind<CoreSo>().FromInstance(_baseCoreSo).AsSingle().NonLazy();
            Container.Bind<ILoadSaveService>().To<JsonToFileLoadSaveService>().AsSingle();
            Container.Bind<GameStateProvider>().AsSingle().NonLazy();
            
            Container.Bind<InputActionAsset>().FromInstance(_inputActionAsset).AsSingle();
            
            //Container.Bind<SettingsUI>().FromInstance(GameEntryPoint.UIRef.SettingsUI).AsSingle();
            //Container.Bind<MessageGameUI>().FromInstance(GameEntryPoint.UIRef.MessageGameUI).AsSingle();
            Container.BindInterfacesAndSelfTo<Settings>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<MessageGame>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<DialogsLanguageManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<LanguageChanger>().AsSingle();
            Container.Bind<LanguageSO[]>().FromInstance(_languagesSO).AsSingle().NonLazy();
            Container.Bind<GameSettingsSO>().FromInstance(_gameSettingsSo).AsSingle().NonLazy();
            Container.Bind<QuestsSO>().FromInstance(_questsSo).AsSingle().NonLazy();

            Container.Bind<TimeManager>().AsSingle();
            Container.Bind<CursorManager>().AsSingle();
        }
    }
}