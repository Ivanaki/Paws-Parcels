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

        public ReactiveProperty<int> CurrentMission { get; }
        public ObservableList<string> ActiveDestinations { get; } = new();
        public ReactiveProperty<int> MessageProgress { get; }
        public ReactiveProperty<bool> HasMissionTimer { get; }
        public ReactiveProperty<float> MissionTimerSeconds { get; }
        //public readonly ReactiveProperty<bool> IsActiveDestination;

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
            MessageProgress = new ReactiveProperty<int>(questData.MessageProgress);
            HasMissionTimer = new ReactiveProperty<bool>(questData.HasMissionTimer);
            MissionTimerSeconds = new ReactiveProperty<float>(questData.MissionTimerSeconds);
            //IsActiveDestination = new ReactiveProperty<bool>(questData.IsActiveDestination);
            
            CurrentMission.Skip(1).Subscribe(v => questData.CurrentMission = v);
            MessageProgress.Skip(1).Subscribe(v => questData.MessageProgress = v);
            HasMissionTimer.Skip(1).Subscribe(v => questData.HasMissionTimer = v);
            MissionTimerSeconds.Skip(1).Subscribe(v => questData.MissionTimerSeconds = v);
            //IsActiveDestination.Skip(1).Subscribe(v => questData.IsActiveDestination = v);

            /*Message = new MessageToCompleteProgressProxy(questData.Message);
            ManyMessages = questData.ManyMessages.Select(x => new MessageToCompleteProgressProxy(x)).ToList();*/
            
            questData.ActiveDestinations.ForEach(name => ActiveDestinations.Add(name));

            ActiveDestinations.ObserveAdd()
                .Subscribe(e => questData.ActiveDestinations.Add(e.Value));

            ActiveDestinations.ObserveRemove()
                .Subscribe(e => questData.ActiveDestinations.Remove(e.Value));
            
            ActiveDestinations.ObserveClear().
                Subscribe(e => questData.ActiveDestinations.Clear());
        }
    }
}
