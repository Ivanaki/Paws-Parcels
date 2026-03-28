using System;

namespace Paws_Parcels.Scripts.Saves.States.GameState
{
    [Serializable]
    public class ItemClearData
    {
        public string ItemTypeId = "";
        public string DestinationId = "";
    }
    
    public class ItemClearProxy
    {
        public ItemClearData Origin;

        public string ItemTypeId
        {
            get => Origin.ItemTypeId;
            set => Origin.ItemTypeId = value;
        }

        public string DestinationId
        {
            get => Origin.DestinationId;
            set => Origin.DestinationId = value;
        }
        
        public ItemClearProxy(ItemClearData itemClearData)
        {
            Origin = itemClearData;
            ItemTypeId = itemClearData.ItemTypeId;
            DestinationId = itemClearData.DestinationId;
        }
    }
}