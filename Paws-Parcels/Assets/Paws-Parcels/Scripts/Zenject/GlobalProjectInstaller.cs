using Game.Root;
using Paws_Parcels.Scripts;
using Paws_Parcels.Scripts.Quests;
using Paws_Parcels.Scripts.Saves.States;
using Paws_Parcels.Scripts.Saves.States.GameState;
using SaveLaod;
using Saves;
using Services;
using UI;
using UI.Screens.Settings;
using UnityEngine;
using Zenject;


namespace MotorHome.Scripts.Zenject
{
    public class GlobalProjectInstaller : MonoInstaller
    {
        [SerializeField] private UIGameView _uiGameViewPrefab;
        
        //[SerializeField] private SettingsSo _baseSettingsSo;
        //[SerializeField] private CoreSo _baseCoreSo;
        
        //[SerializeField] private InputActionAsset _inputActionAsset;

        //[SerializeField] private LanguageSO[] _languagesSO;
        //[SerializeField] private GameSettingsSO _gameSettingsSo;
        //[SerializeField] private QuestsSO _questsSo;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameEntryPoint>().AsSingle().NonLazy();
            Container.BindExecutionOrder<GameEntryPoint>(-1000);

            Container.Bind<UIGameView>().FromComponentInNewPrefab(_uiGameViewPrefab).AsSingle()
                .OnInstantiated<UIGameView>((ctx, ui) =>
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
            
            Container.Bind<JsonFileSerializer>().AsSingle();
            Container.Bind<ScreenshotSaver>().AsSingle();
            
            Container.Bind<LoadService>().AsSingle();
            //Container.Bind<GameStateProvider>().AsSingle();
            Container.Bind<SettingsStateProvider>().AsSingle();
            Container.Bind<SettingsService>().AsSingle();
            
            Container.Bind<GameStateProvider>().AsSingle();
            
            Container.Bind<QuestsService>().AsSingle();
            
            Container.Bind<EditorDataService>().AsSingle();
            
            Container.Bind<MessageToUserService>().AsSingle();
            Container.Bind<GameInputService>().AsSingle();
            Container.Bind<CursorService>().AsSingle();
            Container.Bind<PlayerLocationService>().AsSingle();
            Container.BindInterfacesAndSelfTo<LocalizationService>().AsSingle();
            //Container.Bind<ClosePopupsWithBackButtonService>().AsSingle();
            
            //Old bindings
            
            
            /*Container.Bind<SettingsSo>().FromInstance(_baseSettingsSo).AsSingle().NonLazy();
            Container.Bind<CoreSo>().FromInstance(_baseCoreSo).AsSingle().NonLazy();
            Container.Bind<GameStateProvider>().AsSingle().NonLazy();
            
            Container.Bind<InputActionAsset>().FromInstance(_inputActionAsset).AsSingle();
            
            Container.BindInterfacesAndSelfTo<Settings>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<MessageGame>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<DialogsLanguageManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<LanguageChanger>().AsSingle();
            Container.Bind<LanguageSO[]>().FromInstance(_languagesSO).AsSingle().NonLazy();
            Container.Bind<GameSettingsSO>().FromInstance(_gameSettingsSo).AsSingle().NonLazy();
            Container.Bind<QuestsSO>().FromInstance(_questsSo).AsSingle().NonLazy();

            Container.Bind<TimeManager>().AsSingle();
            Container.Bind<CursorManager>().AsSingle();*/
        }
    }
}