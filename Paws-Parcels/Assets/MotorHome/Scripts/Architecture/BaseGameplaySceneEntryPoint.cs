
using R3;
using UnityEngine;

namespace Game.Root
{
    public abstract class BaseGameplaySceneEntryPoint : MonoBehaviour
    {
        protected Subject<Unit> _restartSignalSub = new();
        protected Subject<Unit> _mainMenuSignalSub = new();
        
        public virtual void GoToMenu()
        {
            _mainMenuSignalSub.OnNext(Unit.Default);
        }

        public virtual void Restart()
        {
            _restartSignalSub.OnNext(Unit.Default);
        }
    }
}
