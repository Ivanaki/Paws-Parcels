using UnityEngine;

namespace Services
{
    public class CursorService
    {
        public void SetCursorStateLocked(bool newState)
        {
            //Debug.Log("SetCursorStateLocked: " +  newState);
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !newState;
        }
    }
}