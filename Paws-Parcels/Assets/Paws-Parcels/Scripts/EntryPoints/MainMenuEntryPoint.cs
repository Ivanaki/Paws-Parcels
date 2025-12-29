using Game.Params;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Root
{
    public class MainMenuEntryPoint : IInitializable
    {
        private readonly MainMenuScenesChanger _mainMenuScenesChanger;
        private readonly GameEntryPoint _gameEntryPoint;

        private MainMenuEntryPoint(MainMenuScenesChanger mainMenuScenesChanger, GameEntryPoint gameEntryPoint)
        {
            _mainMenuScenesChanger = mainMenuScenesChanger;
            _gameEntryPoint = gameEntryPoint;
        }

        public void Initialize()
        {
            Run();
        }
        
        private void Run()
        {
            var baseEnterParams = _gameEntryPoint.GetSceneEnterParams(SceneManager.GetActiveScene().name);
            var mainMenuEnterParams = baseEnterParams == null
                ? new MainMenuEnterParams("", "")
                : baseEnterParams.As<MainMenuEnterParams>();
            
            Debug.Log($"MainMenu ENTRY POINT. Message: {mainMenuEnterParams.MessageToMainMenu}");
            
            
            
            
            
            
            
            
            
            var loadType = LoadType.New;
            var saveNumber = -1;

            _mainMenuScenesChanger.ExitToDesktopSignal.Subscribe(_ => ExitToDesktop());
            
            
            
            
            var coreExitSignalSubject = _mainMenuScenesChanger.CoreSignal.Select(_ =>
                new MainMenuExitParams(Scenes.CORE, 
                    new CoreEnterParams(Scenes.MAINMENU,"from mainMenu", loadType, saveNumber)));
            
            var restartEnter = new MainMenuEnterParams(Scenes.MAINMENU, "");
            var exitParamsRestart = new MainMenuExitParams(Scenes.MAINMENU, restartEnter);
            var exitRestartSubject = _mainMenuScenesChanger.RestartSignal.Select(_ => exitParamsRestart);
            
            var exitSignal = exitRestartSubject.Merge(coreExitSignalSubject);
            
            exitSignal.Subscribe(mainMenuExitParams =>
            {
                _gameEntryPoint.ScenesLoad(mainMenuExitParams.SceneTo, mainMenuExitParams.BaseEnterParams);
            });
            
            _gameEntryPoint.SceneLoaded(SceneManager.GetActiveScene().name);
        }

        private void ExitToDesktop()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
