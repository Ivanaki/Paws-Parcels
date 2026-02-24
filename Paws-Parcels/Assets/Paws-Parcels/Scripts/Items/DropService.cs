using System;
using Services;
using UnityEngine.InputSystem;

namespace Items
{
    public class DropService : IDisposable
    {
        private readonly ItemsSystem _itemsSystem;
        private readonly GameInputService _gameInputService;

        public DropService(ItemsSystem itemsSystem, GameInputService gameInputService)
        {
            _itemsSystem = itemsSystem;
            _gameInputService = gameInputService;
            
            gameInputService.InputSystem.PlayerNotMovement.Drop.performed += Drop;
        }
        
        private void Drop(InputAction.CallbackContext obj = new())
        {
            _itemsSystem.TryDrop();
        }

        public void Dispose()
        {
            _gameInputService.InputSystem.PlayerNotMovement.Drop.performed -= Drop;
        }
    }
}