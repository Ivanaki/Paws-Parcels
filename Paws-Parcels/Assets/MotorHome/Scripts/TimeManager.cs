using UnityEngine;

namespace MotorHome.Scripts
{
    public class TimeManager
    {
        public void Go()
        {
            Time.timeScale = 1f;
        }
        
        public void Stop()
        {
            Time.timeScale = 0f;
        }
    }
}