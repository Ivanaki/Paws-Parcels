using System;

namespace Paws_Parcels.Scripts.Saves.States.GameState
{
    [Serializable]
    public class ItemUpCar
    {
        public string ItemTypeId = "";
        public string DestinationId = "";
        //можно будет добавить что-то еще при физике предметов
    }
    
    public class ItemUpCarProxy
    {
        public ItemUpCar Origin;

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
        
        public ItemUpCarProxy(ItemUpCar itemUpCar)
        {
            Origin = itemUpCar;
            ItemTypeId = itemUpCar.ItemTypeId;
            DestinationId = itemUpCar.DestinationId;
        }
    }
}