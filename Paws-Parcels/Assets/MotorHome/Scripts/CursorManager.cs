using UnityEngine;

namespace MotorHome.Scripts
{
    public class CursorManager
    {
        public void SetCursorStateLocked(bool newState)
        {
            //Debug.Log("SetCursorStateLocked: " +  newState);
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !newState;
        }
    }
}