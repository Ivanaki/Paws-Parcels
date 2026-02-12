using R3;

namespace Paws_Parcels.Scripts.Quests
{
    public class MessageToCompleteProgressProxy
    {
        public readonly MessageToCompleteProgress Origin;
        
        public bool IsCounted
        {
            get => Origin.IsCounted;
            set => Origin.IsCounted = value;
        }
        
        public ReactiveProperty<int> ProgressCount;
        public ReactiveProperty<bool> IsSuccess;

        public MessageToCompleteProgressProxy(MessageToCompleteProgress origin)
        {
            Origin = origin;

            ProgressCount = new ReactiveProperty<int>(Origin.ProgressCount);
            IsSuccess = new ReactiveProperty<bool>(Origin.IsSuccess);
            
            ProgressCount.Skip(1).Subscribe(v => Origin.ProgressCount = v);
            IsSuccess.Skip(1).Subscribe(v => Origin.IsSuccess = v);
        }
    }
}