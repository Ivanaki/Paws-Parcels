using System;
using R3;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace MotorHome.Scripts
{
    public class Bear : MonoBehaviour
    {
        [Inject] private SoloMonolog _soloMonolog;
        
        [SerializeField] private NPCInteractable _npcInteractable;

        [SerializeField] private UnityEvent _unityEvent;
        [SerializeField] private UnityEvent _afterMonolog;

        private IDisposable _disposable;
        
        private void OnEnable()
        {
            _disposable = _npcInteractable._currentReference.Subscribe(v =>
            {
                if (v == 2)
                {
                    _unityEvent.Invoke();
                    _soloMonolog.StartMonolog(new[] { "Коробки внутрь машины не уместятся.", "Положу их на крышу." }).Take(1).Subscribe(_ =>
                    {
                        _afterMonolog.Invoke();
                    });
                }
            });
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }
    }
}