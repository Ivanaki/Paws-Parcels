using System.Collections.Generic;
using Car_Player;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Scripts.Player
{
    public class Doors : BaseInteractable
    {
        [Inject] private DoorService _doorService;
        
        public override void Interact()
        {
            _doorService.ChangeDoor();
        }
    }
}