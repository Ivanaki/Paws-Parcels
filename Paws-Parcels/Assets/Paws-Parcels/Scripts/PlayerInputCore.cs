using System;
using Car_Player;
using Items;
using Paws_Parcels.Scripts.Car_Player;
using Services;
using UnityEngine.InputSystem;
using Zenject;

namespace Paws_Parcels.Scripts
{
    public class PlayerInputCore : IDisposable, IInitializable
    {
        private readonly ItemsSystem _itemsSystem;
        private readonly SitInCarService _sitInCarService;
        private readonly InputSystem_Actions _inputSystem;

        public PlayerInputCore(GameInputService gameInputService, ItemsSystem itemsSystem, 
            SitInCarService sitInCarService)
        {
            _inputSystem = gameInputService.InputSystem;
            _itemsSystem = itemsSystem;
            _sitInCarService = sitInCarService;
        }
        
        public void Initialize()
        {
            _inputSystem.PlayerNotMovement.Drop.performed += Drop;
            _inputSystem.CarNotMovement.Exit.performed += ExitCar;
            
        }
        
        public void Dispose()
        {
            _inputSystem.PlayerNotMovement.Drop.performed -= Drop;
            _inputSystem.CarNotMovement.Exit.performed -= ExitCar;
        }
        
        private void Drop(InputAction.CallbackContext obj = new())
        {
            _itemsSystem.TryDrop();
        }
        
        private void ExitCar(InputAction.CallbackContext obj = new())
        {
            _sitInCarService.TryExit();
        }
    }
}