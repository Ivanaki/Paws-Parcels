using System;
using Game.Params;
using Paws_Parcels.Scripts;
using R3;
using Services;
using UI;
using UI.MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Root
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIMainMenuRootBinder _uiMainMenuRootBinderPrefab;
        
        private MainMenuSignals _mainMenuSignals;
        private GameEntryPoint _gameEntryPoint;
        private UIGameView _uiGameView;
        private UIMainMenuRootViewModel _uiMainMenuRootViewModel;
        private MainMenuUIManager _mainMenuUIManager;
        private LoadService _loadService;
        private MessageToUserService _messageToUserService;
        private GameInputService _gameInputService;
        private CursorService _cursorService;
        private DiContainer _container;
        
        private IDisposable _disposable;

        [Inject]
        private void Construct(MainMenuSignals mainMenuSignals, GameEntryPoint gameEntryPoint,
            UIGameView uiGameView, UIMainMenuRootViewModel uiMainMenuRootViewModel, MainMenuUIManager mainMenuUIManager,
            LoadService loadService, MessageToUserService messageToUserService, GameInputService gameInputService,
            CursorService cursorService, DiContainer container)
        {
            _mainMenuSignals = mainMenuSignals;
            _gameEntryPoint = gameEntryPoint;
            _uiGameView = uiGameView;
            _uiMainMenuRootViewModel = uiMainMenuRootViewModel;
            _mainMenuUIManager = mainMenuUIManager;
            _loadService = loadService;
            _messageToUserService = messageToUserService;
            _gameInputService = gameInputService;
            _cursorService = cursorService;
            _container = container;
        }

        public void Start()
        {
            Run();
        }
        
        private void Run()
        {
            var baseEnterParams = _gameEntryPoint.GetSceneEnterParams(SceneManager.GetActiveScene().name);
            var mainMenuEnterParams = baseEnterParams == null
                ? new MainMenuEnterParams("none", "",false, "")
                : baseEnterParams.As<MainMenuEnterParams>();
            
            Debug.Log($"MainMenu ENTRY POINT. Message: {mainMenuEnterParams.MessageToMainMenu}");
            
            
            InitUI();

            _messageToUserService.SendMessage(mainMenuEnterParams.MessageToUser,
                mainMenuEnterParams.IsWasErrorWhileLoadCoreScene ? Color.red : Color.white);
            
            _cursorService.SetCursorStateLocked(false);
            Time.timeScale = 1f;
            
            /*var loadType = LoadType.New;
            var saveNumber = -1;*/

            _mainMenuSignals.ExitToDesktopSignal.Subscribe(_ => ExitToDesktop());
            
            
            
            var newGameSignalSubject = _mainMenuSignals.NewGameSignal.Select(_ =>
                new MainMenuExitParams(Scenes.CORE, 
                    new CoreEnterParams(Scenes.MAINMENU, LoadType.New, Guid.Empty)));
            
            var loadGameSignalSubject = _loadService.LoadCoreSceneWithSaveId.Select(saveId =>
                new MainMenuExitParams(Scenes.CORE, 
                    new CoreEnterParams(Scenes.MAINMENU, LoadType.LoadCurrentSave, saveId)));
            
            var restartEnter = new MainMenuEnterParams(Scenes.MAINMENU, "", false, "restart");
            var exitParamsRestart = new MainMenuExitParams(Scenes.MAINMENU, restartEnter);
            var exitRestartSubject = _mainMenuSignals.RestartSignal.Select(_ => exitParamsRestart);
            
            var exitSignal = exitRestartSubject.Merge(newGameSignalSubject).Merge(loadGameSignalSubject);
            
            _disposable = exitSignal.Subscribe(mainMenuExitParams =>
            {
                _gameEntryPoint.ScenesLoad(mainMenuExitParams.SceneTo, mainMenuExitParams.BaseEnterParams);
            });
            
            _gameEntryPoint.SceneLoaded(SceneManager.GetActiveScene().name);
        }

        private void InitUI()
        {
            var uiSceneRootBinder = _container.InstantiatePrefabForComponent<UIMainMenuRootBinder>(_uiMainMenuRootBinderPrefab);
            //var uiSceneRootBinder = Instantiate(_uiMainMenuRootBinderPrefab);
            _uiGameView.AttachSceneUI(uiSceneRootBinder.gameObject);
            
            uiSceneRootBinder.Bind(_uiMainMenuRootViewModel);
            
            //открыть HUD
            _mainMenuUIManager.OpenScreenMainMenu();
        }
        
        private void ExitToDesktop()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}
