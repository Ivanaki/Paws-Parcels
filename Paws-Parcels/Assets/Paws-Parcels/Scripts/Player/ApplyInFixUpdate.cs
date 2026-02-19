using UnityEngine;
using R3;
using Zenject;

namespace Paws_Parcels.Scripts.Player
{
    public class ApplyInFixUpdate : IFixedTickable
    {
        private bool _needApply = false;
        private Subject<Unit> _applySubject = new();

        /// <summary>
        /// use Take(1)
        /// </summary>
        /// <returns></returns>
        public Observable<Unit> ObserveApply()
        {
            _needApply = true;
            return _applySubject;
        }

        public void FixedTick()
        {
            if (_needApply)
            {
                _applySubject.OnNext(Unit.Default);
                _needApply = false;
            }
        }
    }
}