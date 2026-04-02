using System.Collections.Generic;
using Paws_Parcels.Scripts.Player;
using R3;
using Services;
using Zenject;

namespace Car_Player
{
    public class CarUpService
    {
        [Inject] private MessageSystem _messageSystem;

        private readonly HashSet<Item> _destinations = new();
        
        public CarUpService()
        {
            _messageSystem.AddListenerWithManyMessages("update_items_up_message").Subscribe(_ =>
            {
                foreach (var item in _destinations)
                {
                    _messageSystem.SendCompositeMessage(this, "item_on_roof:" + item.DestinationId);
                }
            });
        }

        public void AddItem(Item item)
        {
            item.OnDestroyObserve.Take(1).Subscribe(_ =>
            {
                RemoveItem(item);
            });
            
            _destinations.Add(item);
            _messageSystem.SendCompositeMessage(this, "item_on_roof:" + item.DestinationId);
        }

        public void RemoveItem(Item item)
        {
            _destinations.Remove(item);
            _messageSystem.SendCompositeMessage(this, "item_down_roof:" + item.DestinationId);
        }
    }
}