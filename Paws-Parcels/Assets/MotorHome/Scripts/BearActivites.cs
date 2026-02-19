using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class BearActivites : MonoBehaviour
    {
        [Inject]
        private CompassUI _compassUI;
        
        //private 
        
        public void End()
        {
            _compassUI.ActivateCompassUI();
        }
    }
}