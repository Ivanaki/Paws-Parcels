using R3;

namespace Paws_Parcels.Scripts.Saves.States.GameResources
{
    public class ResourceProxy
    {
        public readonly ResourceData Origin;
        
        public ResourcesType ResourcesType => Origin.ResourcesType;
        public ReactiveProperty<int> Amount { get; }

        public ResourceProxy(ResourceData resourceData)
        {
            Origin = resourceData;
            Amount = new ReactiveProperty<int>(resourceData.Amount);
            
            Amount.Skip(1).Subscribe(v => resourceData.Amount = v);
        }
    }
}