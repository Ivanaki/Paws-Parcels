using System;
using UI;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class AnotherSceneScript : MonoBehaviour
    {
        private CoreUI _coreUI;
        private UIGameView _uiGameView;
        private Settings _settings;
        private CoreSaver _coreSaver;
        private UIActiveCoreManager _uiActiveCoreManager;
        
        [Inject]
        private void Construct(UIGameView uiGameView, CoreUI coreUI, Settings settings, CoreSaver coreSaver,
            UIActiveCoreManager uiActiveCoreManager)
        {
            _uiGameView  = uiGameView;
            _coreUI  = coreUI;
            _settings = settings;
            _coreSaver = coreSaver;
            _uiActiveCoreManager = uiActiveCoreManager;
        }

        private void Awake()
        {
            _coreUI.Helper.Inject(_uiActiveCoreManager);
            
            _uiGameView.AttachSceneUI(_coreUI.gameObject);
            
            _settings.BindOpenSettingsObservable(_coreUI.SettingsPressed, _uiActiveCoreManager);
            
            _coreSaver.Start(LoadType.New, -1);
        }
    }
}