using Car_Player;
using Destinations;
using Dialogue;
using Game.Root;
using Items;
using Map;
using Messages;
using NPCDialogs;
using Paws_Parcels.NPCDialogs;
using Paws_Parcels.Scripts;
using Paws_Parcels.Scripts.Car_Player;
using Paws_Parcels.Scripts.Car_Player.Respawn;
using Paws_Parcels.Scripts.Metrics;
using Paws_Parcels.Scripts.Player;
using Paws_Parcels.Scripts.Quests;
using Paws_Parcels.Scripts.Saves.States;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Quests;
using SaveLaod;
using Saves;
using Services;
using UI;
using UI.HUD;
using UI.Screens.Settings;
using UnityEngine;
using Zenject;


namespace MotorHome.Scripts.Installers
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
            Container.BindInterfacesAndSelfTo<SessionMetricsService>().AsSingle().NonLazy();
            Container.Bind<ScreenshotSaver>().AsSingle();
            Container.Bind<LoadService>().AsSingle();
            Container.Bind<SettingsService>().AsSingle();
            Container.Bind<RewardService>().AsSingle();
            Container.Bind<MessageToUserService>().AsSingle();
            Container.Bind<CursorService>().AsSingle();
            Container.Bind<InteractablesLocationsService>().AsSingle();
            Container.Bind<MessageSystem>().AsSingle();
            Container.Bind<QuestsDatabase>().AsSingle();
            Container.Bind<EditorDataService>().AsSingle();
            Container.Bind<UiObjectsDatabase>().AsSingle();
            Container.Bind<CanvasUiVisualizer>().AsSingle();
            Container.Bind<ItemsDatabase>().AsSingle();
            Container.Bind<ItemsSystem>().AsSingle();
            Container.Bind<DropSettings>().AsSingle();
            Container.Bind<SitInCarService>().AsSingle();
            Container.Bind<DoorService>().AsSingle();
            Container.Bind<LocalizationService>().AsSingle();
            Container.Bind<PlayerService>().AsSingle();
            Container.Bind<CutsceneService>().AsSingle();
            Container.Bind<PlayerShowableService>().AsSingle();
            Container.Bind<ShowableInterfaceService>().AsSingle();
            Container.Bind<HUDService>().AsSingle();
            
            Container.Bind<ResourcesService>().AsSingle();
            Container.Bind<DialogueRegistrations>().AsSingle();
            Container.Bind<DialogueInQuestsService>().AsSingle();
            Container.Bind<ItemDestinationPutterService>().AsSingle();
            Container.Bind<MapUiVisualizer>().AsSingle();
            Container.Bind<MapObjectsDatabase>().AsSingle();
            Container.Bind<BuildingsLogic>().AsSingle();
            Container.Bind<CarUpService>().AsSingle();
            Container.Bind<RespawnService>().AsSingle();
            Container.Bind<MapSetMarked>().AsSingle();
            
            //
            Container.Bind<GameStateProvider>().AsSingle();
            Container.Bind<GameProxy>().FromResolveGetter<GameStateProvider>(x => x.GameProxy).AsSingle();
            
            //NonLazy
            Container.Bind<BaseDialogueRegistrations>().AsSingle().NonLazy();
            Container.Bind<QuestsService>().AsSingle().NonLazy();
            Container.Bind<SettingsStateProvider>().AsSingle().NonLazy();
            Container.Bind<DestinationService>().AsSingle().NonLazy();
            Container.Bind<HintsService>().AsSingle().NonLazy();
            Container.Bind<CarStopper>().AsSingle().NonLazy();
            Container.Bind<DialogueService>().AsSingle().NonLazy();
            



            
            //в global bindings нет смысла биндить интерфейсы IDisposable
            Container.BindInterfacesAndSelfTo<ApplyInFixUpdate>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameInputService>().AsSingle();
            Container.BindInterfacesAndSelfTo<PutInCarItemsService>().AsSingle().NonLazy();
            
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
