using System.Collections.Generic;
using System.Linq;
using ObservableCollections;
using R3;

namespace Paws_Parcels.Scripts.Quests
{
    public class QuestDataProxy
    {
        public readonly QuestData Origin;
        
        public string Id => Origin.Id;

        public readonly ReactiveProperty<int> CurrentMission;
        public readonly ReactiveProperty<bool> IsActiveDestination;

        /*public bool IsManyMessages
        {
            get => Origin.IsManyMessages;
            set => Origin.IsManyMessages = value;
        }

        public MessageToCompleteProgressProxy Message;
        public List<MessageToCompleteProgressProxy> ManyMessages;*/

        //public ObservableList<string> CurrentActiveDestinations { get; } = new();

        public QuestDataProxy(QuestData questData)
        {
            Origin = questData;
            CurrentMission = new ReactiveProperty<int>(questData.CurrentMission);
            IsActiveDestination = new ReactiveProperty<bool>(questData.IsActiveDestination);
            
            CurrentMission.Skip(1).Subscribe(v => questData.CurrentMission = v);
            IsActiveDestination.Skip(1).Subscribe(v => questData.IsActiveDestination = v);

            /*Message = new MessageToCompleteProgressProxy(questData.Message);
            ManyMessages = questData.ManyMessages.Select(x => new MessageToCompleteProgressProxy(x)).ToList();*/
            
            /*questData.CurrentActiveDestinations.ForEach(name => CurrentActiveDestinations.Add(name));

            CurrentActiveDestinations.ObserveAdd()
                .Subscribe(e => questData.CurrentActiveDestinations.Add(e.Value));

            CurrentActiveDestinations.ObserveRemove()
                .Subscribe(e => questData.CurrentActiveDestinations.Remove(e.Value));
            
            CurrentActiveDestinations.ObserveClear().
                Subscribe(e => questData.CurrentActiveDestinations.Clear());*/
        }
    }
}