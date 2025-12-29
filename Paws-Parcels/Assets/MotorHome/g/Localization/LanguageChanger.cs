using System;
using MotorHome.Scripts.Saves;
using R3;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class LanguageChanger : IInitializable, IDisposable
    {
        //public InterfaceTextData InterfaceTextData { private set; get; } = null;
        public ReadOnlyReactiveProperty<InterfaceTextData> OnLanguageChanged => _languageChanged;
        
        private readonly DialogsLanguageManager _dialogsLanguageManager;
        private readonly GameStateProvider _gameStateProvider;
        private readonly ReactiveProperty<InterfaceTextData> _languageChanged = new();
        
        private CompositeDisposable _disposable;
        private IDisposable _settingsStateProxyUpdated;
        
        public LanguageChanger(DialogsLanguageManager dialogsLanguageManager, GameStateProvider  gameStateProvider)
        {
            _dialogsLanguageManager = dialogsLanguageManager;
            _gameStateProvider = gameStateProvider;
        }

        public (bool, QuestData) GetQuestData(QuestEnum quest) => _dialogsLanguageManager.GetQuestData(
            _gameStateProvider.SettingsStateProxy.CurrentValue.Language.CurrentValue, quest);
        
        public PhrasesOneManData[] GetDialog(DialogEnum dialog) =>
            _dialogsLanguageManager.GetDialog(_gameStateProvider.SettingsStateProxy.CurrentValue.Language.CurrentValue, dialog);
        
        
        public void Initialize()
        {
            //Debug.Log("Initialize");
            
            _disposable = new CompositeDisposable();
            IDisposable dis;
            
            dis = _gameStateProvider.SettingsStateProxy.Subscribe(settingsStateProxy =>
            {
                _settingsStateProxyUpdated?.Dispose();
                
                if(settingsStateProxy == null) return;
                
                _settingsStateProxyUpdated = settingsStateProxy.Language.Subscribe(value =>
                {
                    var a = _dialogsLanguageManager.GetInterfaceTextData(value);
                    if (a == null)
                        Debug.LogWarning("No interface text data found for " + value);
                    _languageChanged.OnNext(a);
                });
            }); _disposable.Add(dis);
        }
        
        public void Dispose()
        {
            _disposable?.Dispose();
            _settingsStateProxyUpdated?.Dispose();
        }
    }
}