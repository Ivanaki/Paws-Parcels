using UnityEngine;
using System;
using R3;
using Zenject;


namespace MotorHome.Scripts
{
    public abstract class BaseLanguageInterface : MonoBehaviour
    {
        private LanguageChanger _languageChanger;
        private IDisposable _disposable;
        
        [Inject]
        private void Construct(LanguageChanger languageChanger)
        {
            _languageChanger = languageChanger;
        }

        protected abstract void LanguageChange(InterfaceTextData interfaceTextData);
        
        private void OnEnable()
        {
            //print("OnEnable");
            _disposable = _languageChanger.OnLanguageChanged.Subscribe(data =>
            {
                if (data == null) return;
                LanguageChange(data);
            });
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }
        
    }
}