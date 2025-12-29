using UnityEngine;
using R3;

namespace MotorHome.Scripts
{
    public class ApplyInFixUpdate : MonoBehaviour
    {
        private bool _needApply = false;
        private Subject<Unit> _applySubject = new();

        public Observable<Unit> Apply()
        {
            _needApply = true;
            return _applySubject;
        }
        
        private void FixedUpdate()
        {
            if (_needApply)
            {
                _applySubject.OnNext(Unit.Default);
                _needApply = false;
            }
        }
    }
}