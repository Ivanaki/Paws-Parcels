using System;
using Game.Params;
using Paws_Parcels.Scripts;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Services;
using UI;
using UI.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Root
{
    public class CoreEntryPoint : MonoBehaviour
    {
        [SerializeField] private UICoreRootBinder _uiCoreRootBinderPrefab;
        
        private CoreSignals _coreSignals;
        private GameEntryPoint _gameEntryPoint;
        private UIGameView _uiGameView;
        private UICoreRootViewModel _uiCoreRootViewModel;
        private CoreUIManager _coreUIManager;
        private GameStateProvider _gameStateProvider;
        private LoadService _loadService;
        private GameInputService _gameInputService;
        private CursorService _cursorService;
        private DiContainer _container;

        private IDisposable _disposable;

        [Inject]
        private void Construct(CoreSignals coreSignals, GameEntryPoint gameEntryPoint,
            UIGameView uiGameView, UICoreRootViewModel uiCoreRootViewModel, CoreUIManager coreUIManager,
            GameStateProvider gameStateProvider, LoadService loadService, GameInputService  gameInputService,
            CursorService cursorService, DiContainer container)
        {
            _coreSignals = coreSignals;
            _gameEntryPoint = gameEntryPoint;
            _uiGameView = uiGameView;
            _uiCoreRootViewModel = uiCoreRootViewModel;
            _coreUIManager = coreUIManager;
            _gameStateProvider = gameStateProvider;
            _loadService = loadService;
            _gameInputService = gameInputService;
            _cursorService = cursorService;
            _container = container;
        }
        
        //последовательность методов: Awake -> OnEnable -> IInitializable -> Start
        
        private void Awake()
        {
            
        }

        private void Start()
        {
            Run();
        }
        
        private void Run()
        {
            var baseEnterParams = _gameEntryPoint.GetSceneEnterParams(SceneManager.GetActiveScene().name);
            var coreEnterParams = baseEnterParams == null
                ? new CoreEnterParams("", LoadType.New, Guid.Empty)
                : baseEnterParams.As<CoreEnterParams>();
            
            Debug.Log($"Core ENTRY POINT LoadType: {coreEnterParams.LoadType} ");
            
            var result = TryLoadGame(coreEnterParams);
            if (result.Item1 == false)
            {
                var mainMenuEnterParamsWhileLoadError = new MainMenuEnterParams(Scenes.CORE, 
                    "", true, result.Item2);
                _gameEntryPoint.ScenesLoad(Scenes.MAINMENU, mainMenuEnterParamsWhileLoadError);
                return;
            }
            
            InitUI();
            
            _gameInputService.InputSystem.UI.Disable();
            _gameInputService.InputSystem.Gameplay.Enable();
            _gameInputService.InputSystem.Car.Disable();
            _gameInputService.InputSystem.Player.Enable();
            _gameInputService.InputSystem.PlayerNotMovement.Enable();
            _gameInputService.InputSystem.CarNotMovement.Enable();
            _cursorService.SetCursorStateLocked(true);
            Time.timeScale = 1f;
            
            
            
            //TODO с четверга:
            //сделать ui и систему Quests
            //далее переходить к самим механикам на сцене
            
            
            
            //обсёрваблы выхода из сцены:
            
            var loadGameSignalSubject = _loadService.LoadCoreSceneWithSaveId.Select(saveId =>
                new CoreExitParams(Scenes.CORE, 
                    new CoreEnterParams(Scenes.CORE, LoadType.LoadCurrentSave, saveId)));
            
            var mainMenuEnter = new MainMenuEnterParams(Scenes.CORE, "", false, "");
            var exitParamsMainMenu = new CoreExitParams(Scenes.MAINMENU, mainMenuEnter);
            var exitMainMenuSubject = _coreSignals.MainMenuSignal.Select(_ => exitParamsMainMenu);
            
            var restartEnter = new CoreEnterParams(Scenes.CORE, LoadType.Continue, Guid.Empty);
            var exitParamsRestart = new CoreExitParams(Scenes.CORE, restartEnter);
            var exitRestartSubject = _coreSignals.RestartSignal.Select(_ => exitParamsRestart);
            
            var exitSignal = exitMainMenuSubject.Merge(exitRestartSubject).Merge(loadGameSignalSubject);

            _disposable = exitSignal.Subscribe(coreExitParams =>
            {
                _gameEntryPoint.ScenesLoad(coreExitParams.SceneTo, coreExitParams.BaseEnterParams);
            });
            
            _gameEntryPoint.SceneLoaded(SceneManager.GetActiveScene().name);
        }

        private void InitUI()
        {
            var uiSceneRootBinder = _container.InstantiatePrefabForComponent<UICoreRootBinder>(_uiCoreRootBinderPrefab);
            //var uiSceneRootBinder = Instantiate(_uiCoreRootBinderPrefab);
            _uiGameView.AttachSceneUI(uiSceneRootBinder.gameObject);
            
            uiSceneRootBinder.Bind(_uiCoreRootViewModel);

            //var coreBackService = new CoreBackService(,_coreUIManager);
            
            //открыть HUD
            //_coreUIManager.OpenScreenPause();
        }

        private (bool, string) TryLoadGame(CoreEnterParams coreEnterParams)
        {
            var errorFlag = false;
            var errorMessageToUser = "";
            
            //загрузить игру
            if (coreEnterParams.LoadType == LoadType.New)
            {
                var result = _gameStateProvider.LoadNewGame();
                if (result)
                {
                    print("Загружена новая игра из настроек проекта: Resources/BaseStates/BaseGameSO");
                }
                else
                {
                    errorFlag = true;
                    errorMessageToUser = "Ошибка при загрузке новой игры";
                    Debug.LogError(errorMessageToUser);
                }
            }
            else if (coreEnterParams.LoadType == LoadType.LoadCurrentSave)
            {
                var callback = _gameStateProvider.LoadGameState(coreEnterParams.SaveNumber);
                if (callback.IsSuccess)
                {
                    //print($"Загружено сохранение {coreEnterParams.SaveName}");
                }
                else
                {
                    //при результате FileNotFound callback.Exception = null
                    
                    //Debug.LogError(callback.Exception.Message);
                    errorFlag = true;
                    //errorMessageToUser = $"Ошибка при загрузке сохранения {coreEnterParams.SaveName}";
                    errorMessageToUser = callback.MessageToUser;
                }
            }
            else if (coreEnterParams.LoadType == LoadType.Continue)
            {
                var callback = _gameStateProvider.LoadLastSave();
                if (callback.IsSuccess)
                {
                    print("Загружено последнее сохранение");
                }
                else
                {
                    Debug.LogError(callback.Exception.Message);
                    errorFlag = true;
                    errorMessageToUser = callback.MessageToUser;
                }
            }

            return (!errorFlag, errorMessageToUser);
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}
