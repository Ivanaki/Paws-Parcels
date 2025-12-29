using System;
using Game.Params;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Root
{
    public class CoreEntryPoint : IInitializable
    {
        private readonly CoreScenesChanger _coreScenesChanger;
        private readonly GameEntryPoint _gameEntryPoint;

        private CoreEntryPoint(CoreScenesChanger coreScenesChanger, GameEntryPoint gameEntryPoint)
        {
            _coreScenesChanger = coreScenesChanger;
            _gameEntryPoint = gameEntryPoint;
        }

        public void Initialize()
        {
            Run();
        }
        
        private void Run()
        {
            var baseEnterParams = _gameEntryPoint.GetSceneEnterParams(SceneManager.GetActiveScene().name);
            var coreEnterParams = baseEnterParams == null
                ? new CoreEnterParams("", "", LoadType.New, -1)
                : baseEnterParams.As<CoreEnterParams>();
            
            Debug.Log($"Core ENTRY POINT: {coreEnterParams.DataCore}. LoadType: {coreEnterParams.LoadType} ");
            
            
            
            
            
            
            
            
            var mainMenuEnter = new MainMenuEnterParams(Scenes.CORE, "");
            var exitParamsMainMenu = new CoreExitParams(Scenes.MAINMENU, mainMenuEnter);
            var exitMainMenuSubject = _coreScenesChanger.MainMenuSignal.Select(_ => exitParamsMainMenu);
            
            var restartEnter = new CoreEnterParams(Scenes.CORE, "", LoadType.New, -1);
            var exitParamsRestart = new CoreExitParams(Scenes.CORE, restartEnter);
            var exitRestartSubject = _coreScenesChanger.RestartSignal.Select(_ => exitParamsRestart);
            
            var exitSignal = exitMainMenuSubject.Merge(exitRestartSubject);

            exitSignal.Subscribe(coreExitParams =>
            {
                _gameEntryPoint.ScenesLoad(coreExitParams.SceneTo, coreExitParams.BaseEnterParams);
            });
            
            _gameEntryPoint.SceneLoaded(SceneManager.GetActiveScene().name);
        }
    }
}
