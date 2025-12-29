using System;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class AnotherSceneScript : MonoBehaviour
    {
        private CoreUI _coreUI;
        private UIRootView _uiRootView;
        private Settings _settings;
        private CoreSaver _coreSaver;
        private UIActiveCoreManager _uiActiveCoreManager;
        
        [Inject]
        private void Construct(UIRootView uiRootView, CoreUI coreUI, Settings settings, CoreSaver coreSaver,
            UIActiveCoreManager uiActiveCoreManager)
        {
            _uiRootView  = uiRootView;
            _coreUI  = coreUI;
            _settings = settings;
            _coreSaver = coreSaver;
            _uiActiveCoreManager = uiActiveCoreManager;
        }

        private void Awake()
        {
            _coreUI.Helper.Inject(_uiActiveCoreManager);
            
            _uiRootView.AttachSceneUI(_coreUI.gameObject);
            
            _settings.BindOpenSettingsObservable(_coreUI.SettingsPressed, _uiActiveCoreManager);
            
            _coreSaver.Start(LoadType.New, -1);
        }
    }
}