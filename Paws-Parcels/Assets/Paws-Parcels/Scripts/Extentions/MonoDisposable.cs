using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Extentions
{
    public abstract class MonoDisposable : MonoBehaviour
    {
        protected CompositeDisposable _disposable;
        
        protected virtual void OnEnable()
        {
            _disposable = new CompositeDisposable();
        }

        protected virtual void OnDisable()
        {
            _disposable?.Dispose();
        }
    }
}