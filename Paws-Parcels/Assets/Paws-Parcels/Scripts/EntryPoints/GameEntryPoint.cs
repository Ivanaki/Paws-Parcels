using System;
using System.Collections;
using Game.Params;
using MotorHome.Scripts;
using Paws_Parcels.Scripts.Saves.States;
using R3;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Root
{
    public class GameEntryPoint : IInitializable
    {
        private readonly UIGameView _uiGameView;
        private readonly Coroutines _coroutines;
        private readonly SettingsStateProvider _settingsStateProvider;

        private BaseEnterParams _currentBaseEnterParams = null;
        private string _currentLoadingScene = string.Empty;

        private GameEntryPoint(UIGameView uiGameView, Coroutines coroutines, SettingsStateProvider settingsStateProvider)
        {
            _uiGameView = uiGameView;
            _coroutines = coroutines;
            _settingsStateProvider = settingsStateProvider;
        }

        //initialize перед всеми остальными
        public void Initialize()
        {
            var result = _settingsStateProvider.LoadSettingsState();
            if (!result)
            {
                Debug.Log($"Failed to load settings state => creating baseSettingsState");
                _settingsStateProvider.ResetSettingsState();
            }
            
            RunGame();
        }
        
        private void RunGame()
        {
            var nameScene = Scenes.MAINMENU;
#if UNITY_EDITOR
            nameScene = SceneManager.GetActiveScene().name;

            if ( nameScene != Scenes.MAINMENU && nameScene != Scenes.CORE && nameScene != Scenes.BOOT)
            {
                return;
            }
            
            if (nameScene == Scenes.MAINMENU)
            {
                _currentBaseEnterParams = new MainMenuEnterParams("", "");
                _currentLoadingScene = Scenes.MAINMENU;
                return;
            }
            
            if (nameScene == Scenes.CORE)
            {
                _currentBaseEnterParams = new CoreEnterParams("", "", LoadType.Continue, Guid.Empty);
                _currentLoadingScene = Scenes.CORE;
                return;
            }
            
            if (nameScene == Scenes.BOOT)
            {
                nameScene = Scenes.MAINMENU;
            }
#endif
            ScenesLoad(nameScene);
        }
        
        public void ScenesLoad(string sceneName, BaseEnterParams baseEnterParams = null)
        {
            _currentBaseEnterParams = baseEnterParams;
            _currentLoadingScene = sceneName;
            
            switch (sceneName)
            {
                case Scenes.MAINMENU:
                    _coroutines.StartCoroutine(LoadAndStartMainMenu());
                    break;

                case Scenes.CORE: 
                    _coroutines.StartCoroutine(LoadAndStartCore());
                    break;
                
                default:
                    Debug.LogError("Unknown scene name");
                    break;
            }
        }
        
        private IEnumerator LoadAndStartMainMenu()
        {
            _uiGameView.ShowLoadingScreen();
            
            yield return LoadScene(Scenes.BOOT);
            yield return LoadScene(Scenes.MAINMENU);
            Debug.Log($"Scene {_currentLoadingScene} is loaded");
        }
        
        
        private IEnumerator LoadAndStartCore()
        {
            _uiGameView.ShowLoadingScreen();
            
            yield return LoadScene(Scenes.BOOT);
            yield return LoadScene(Scenes.CORE);
            Debug.Log($"Scene {_currentLoadingScene} is loaded");
        }
        
        
        private IEnumerator LoadScene(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }
        
        public BaseEnterParams GetSceneEnterParams(string sceneName)
        {
            if (sceneName != _currentLoadingScene)
            {
                Debug.Log($"Scene {_currentLoadingScene} is bug");
            }

            if (_currentBaseEnterParams == null)
            {
                Debug.Log($"Scene {_currentLoadingScene} is bug");
            }
            
            var cur =  _currentBaseEnterParams;
            _currentBaseEnterParams = null;
            
            return cur;
        }
        
        
        public void SceneLoaded(string sceneName)
        {
            if (sceneName != _currentLoadingScene)
            {
                Debug.Log($"Scene {_currentLoadingScene} is bug");
            }

            _currentLoadingScene = string.Empty;
            
            _uiGameView.HideLoadingScreen();
        }
    }
    
    public class Coroutines : MonoBehaviour { }
}