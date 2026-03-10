using System;

namespace Paws_Parcels.Scripts.Saves.States.GameState
{
    [Serializable]
    public class ItemInsideCar
    {
        public string ItemTypeId = "";
        public string DestinationId = "";
    }
    
    public class ItemInsideCarProxy
    {
        public ItemInsideCar Origin;

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
        
        public ItemInsideCarProxy(ItemInsideCar itemInsideCar)
        {
            Origin = itemInsideCar;
            ItemTypeId = itemInsideCar.ItemTypeId;
            DestinationId = itemInsideCar.DestinationId;
        }
    }
}