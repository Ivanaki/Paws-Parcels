using System.Collections.Generic;
using Items;
using Paws_Parcels.Scripts.Player;
using UnityEngine;
using Zenject;

namespace Car_Player
{
    public class BedInteractable : BaseInteractable
    {
        [Inject] private ItemsSystem _itemsSystem;
        
        public override void Interact()
        {
            _itemsSystem.TryPutCat();
        }
    }
}