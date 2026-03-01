using Paws_Parcels.Scripts.Saves.States.GameState;
using UnityEngine;

namespace Items
{
    public class ItemProxy
    {
        public readonly ItemData Origin;
        
        public ItemProxy(ItemData origin)
        {
            //if(origin == null) Debug.LogError("ItemProxy::ItemProxy::Origin is null");
            Origin = origin;
        }

        public string ItemTypeId { set => Origin.ItemTypeId = value; get => Origin.ItemTypeId; }
        public string DestinationId { set => Origin.DestinationId = value; get => Origin.DestinationId; }
        public Vector3 Position { set => Origin.Position = value; get => Origin.Position; }
        public Quaternion Rotation { set => Origin.Rotation = value; get => Origin.Rotation; }
        public Vector3 LinearVelocity { set => Origin.LinearVelocity = value; get => Origin.LinearVelocity; }
        public Vector3 AngularVelocity { set => Origin.AngularVelocity = value; get => Origin.AngularVelocity; }
    }
}