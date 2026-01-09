using System;
using Game.Params;
using R3;
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
        
        private CoreScenesChanger _coreScenesChanger;
        private GameEntryPoint _gameEntryPoint;
        private UIGameView _uiGameView;
        private UICoreRootViewModel _uiCoreRootViewModel;
        private CoreUIManager _coreUIManager;

        [Inject]
        private void Construct(CoreScenesChanger coreScenesChanger, GameEntryPoint gameEntryPoint,
            UIGameView uiGameView, UICoreRootViewModel uiCoreRootViewModel, CoreUIManager coreUIManager)
        {
            _coreScenesChanger = coreScenesChanger;
            _gameEntryPoint = gameEntryPoint;
            _uiGameView = uiGameView;
            _uiCoreRootViewModel = uiCoreRootViewModel;
            _coreUIManager = coreUIManager;
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
                ? new CoreEnterParams("", "", LoadType.New, Guid.Empty)
                : baseEnterParams.As<CoreEnterParams>();
            
            Debug.Log($"Core ENTRY POINT: {coreEnterParams.DataCore}. LoadType: {coreEnterParams.LoadType} ");
            
            InitUI();
            
            
            
            //обсёрваблы выхода из сцены:
            var mainMenuEnter = new MainMenuEnterParams(Scenes.CORE, "");
            var exitParamsMainMenu = new CoreExitParams(Scenes.MAINMENU, mainMenuEnter);
            var exitMainMenuSubject = _coreScenesChanger.MainMenuSignal.Select(_ => exitParamsMainMenu);
            
            var restartEnter = new CoreEnterParams(Scenes.CORE, "", LoadType.Continue, Guid.Empty);
            var exitParamsRestart = new CoreExitParams(Scenes.CORE, restartEnter);
            var exitRestartSubject = _coreScenesChanger.RestartSignal.Select(_ => exitParamsRestart);
            
            var exitSignal = exitMainMenuSubject.Merge(exitRestartSubject);

            exitSignal.Subscribe(coreExitParams =>
            {
                _gameEntryPoint.ScenesLoad(coreExitParams.SceneTo, coreExitParams.BaseEnterParams);
            });
            
            _gameEntryPoint.SceneLoaded(SceneManager.GetActiveScene().name);
        }

        private void InitUI()
        {
            var uiSceneRootBinder = Instantiate(_uiCoreRootBinderPrefab);
            _uiGameView.AttachSceneUI(uiSceneRootBinder.gameObject);
            
            uiSceneRootBinder.Bind(_uiCoreRootViewModel);
            
            //открыть HUD
            _coreUIManager.OpenScreenPause();
        }
    }
}
