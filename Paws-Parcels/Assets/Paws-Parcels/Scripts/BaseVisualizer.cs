using System;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Scripts
{
    public abstract class BaseVisualizer : MonoBehaviour
    {
        protected CompositeDisposable _disposable;
        protected GameProxy _gameProxy;

        [Inject]
        private void Construct(GameStateProvider gameStateProvider)
        {
            _gameProxy = gameStateProvider.GameProxy;
        }
        
        protected virtual void OnEnable()
        {
            _disposable = new CompositeDisposable();
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }
    }
}