using UnityEngine;
using UnityEngine.Events;

namespace Paws_Parcels.Scripts.CarMovement.Ramming
{
    public class RammableObject : MonoBehaviour, IRammable
    {
        [SerializeField] private float _minImpactSpeed = 5f;
        [SerializeField] private int _maxHp = 3;
        [SerializeField] private UnityEvent<int> _onHit;
        [SerializeField] private UnityEvent _onDestroyed;

        public float MinImpactSpeed => _minImpactSpeed;

        private int _currentHp;

        private void Awake()
        {
            _currentHp = _maxHp;
        }

        public void OnRammed(float impactSpeed, Vector3 impactDirection)
        {
            if (_currentHp <= 0)
                return;

            _currentHp--;
            _onHit?.Invoke(_currentHp);

            if (_currentHp <= 0)
                _onDestroyed?.Invoke();
        }
    }
}
