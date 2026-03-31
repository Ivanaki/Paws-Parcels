using System.Collections.Generic;
using Paws_Parcels.Extentions;
using Paws_Parcels.Scripts.Player;
using Quests;
using R3;
using Services;
using UnityEngine;
using Zenject;

namespace Destinations
{
    [RequireComponent(typeof(Collider))]
    public class ItemPutterTriggerZone : InWorldPointDestination
    {
        [Header("ItemPutterTriggerZone settings")] 
        [SerializeField] private string _itemDestinationPutterId;

        private readonly HashSet<string> _items = new();

        [Inject] private MessageSystem _messageSystem;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ColliderRedirector.TryGet(other.gameObject, out Item item))
            {
                var addResult = _items.Add(item.DestinationId);
                if (addResult)
                {
                    item.OnDestroyObserve.Take(1).Subscribe(_ => Remove(item));
                    //print($"Added item: {item.DestinationId}");
                    //послать сообщение: putterId_put:itemDestinationId
                    _messageSystem.SendCompositeMessage(this, _itemDestinationPutterId + "-put:" + item.DestinationId);
                }
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (ColliderRedirector.TryGet(other.gameObject, out Item item))
            {
                Remove(item);
            }
        }

        private void Remove(Item item)
        {
            var removeResult = _items.Remove(item.DestinationId);
            if (removeResult)
            {
                //print($"Removed item: {item.DestinationId}");
                //послать сообщение: putterId_take:itemDestinationId
                _messageSystem.SendCompositeMessage(this, _itemDestinationPutterId + "-take:" + item.DestinationId);
            }
        }
    }
}