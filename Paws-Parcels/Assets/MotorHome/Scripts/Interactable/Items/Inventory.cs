using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MotorHome.Scripts
{
    public class Inventory
    {
        private readonly ShelfInventoryView _view;
        private readonly Item.ItemPlace _place;
        
        private Queue<Item> _items = new();
        
        public Inventory(ShelfInventoryView view, Item.ItemPlace  place)
        {
            _view = view;
            _place = place;
        }

        public void SetSavedItemsInside(Item[] items)
        {
            _items =  new Queue<Item>(items);
            _view.UpdateItems(_items);
        }
        
        public int GetItemNumber(Item item)
        {
            var items = _items.ToList();
            if (items.Contains(item))
            {
                return items.IndexOf(item);
            }
            return -1;
        }
        
        public bool TryAddItem(Item item)
        {
            if (_items.Count + 1 > _view.MaxItems)
            {
                return false;
            }

            item.Place = _place;
            _items.Enqueue(item);
            _view.UpdateItems(_items);
            return true;
        }

        public bool TryGetItem(out Item item)
        {
            if (_items.Count == 0)
            {
                item = null;
                return false;
            }
            item = _items.Dequeue();
            _view.UpdateItems(_items);
            return true;
        }
    }
}