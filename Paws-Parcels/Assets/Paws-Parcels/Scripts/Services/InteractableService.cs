using System;
using MotorHome.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Services
{
    public class InteractableService : IDisposable
    {
        private static readonly LayerMask _layerMask = LayerMask.GetMask("Player");
        
        //private readonly InputAction _interact;

        public InteractableService()
        {
            //_interact = InputSystem.actions.FindAction(ActionMaps.AllState + "/Interact");
            //_interact.performed += Interact;
        }

        private void Interact(InputAction.CallbackContext obj)
        {
            
        }
        
        public void Dispose()
        {
            //_interact.performed -= Interact;
        }
    }
}