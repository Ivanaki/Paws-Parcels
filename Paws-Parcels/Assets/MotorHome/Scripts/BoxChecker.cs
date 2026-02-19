using UnityEngine;
using UnityEngine.Events;

namespace MotorHome.Scripts
{
    public class BoxChecker : MonoBehaviour
    {
        [SerializeField] private UnityEvent _done;

        private int _counter = 0;
        
        public void Plus()
        {
            _counter++;
            if (_counter == 3)
            {
                _done.Invoke();
            }
        }
    }
}