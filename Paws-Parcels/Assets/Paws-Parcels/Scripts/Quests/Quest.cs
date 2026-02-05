using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Paws_Parcels.Scripts.Quests
{
    [Serializable]
    public class RewardReference 
    {
        [field: SerializeField] public Reward Reward { get; private set; }
        [field: SerializeField] public int Count { get; private set; }
    }
    
    /*[Serializable]
    public class BoolObject<T>
    {
        [field: SerializeField] public bool Active { get; private set; }
        [field: SerializeField] public T Value { get; private set; }
    }*/
    
    [Serializable]
    public class Mission
    {
        [field: SerializeField] public string TextCode { get; private set; }
        
        [field: SerializeField, Header("Destination")] 
        public bool HasDestination { get; private set; } = true;

        [field: SerializeField, ShowIf(nameof(HasDestination))]
        public string DestinationCode { get; private set; }

        [field: SerializeField, ShowIf(nameof(HasDestination))]
        public Vector3 DestinationPosition { get; private set; }

        [SerializeField, Header("MessageParameter"), AllowNesting] 
        public bool IsIntEvent  = false;
        
        [field: SerializeField, HideIf("IsIntEvent"), AllowNesting]
        public string CompleteBusMessageParameter { get; private set; } = "Test_message:Test_parameter";
        
        [SerializeField, ShowIf("IsIntEvent"), AllowNesting] 
        private List<string> CompleteBusMessageParametersIntEvent  = new() {"Test_message:Test_parameter"};
        [field: SerializeField, ShowIf("IsIntEvent"), AllowNesting] 
        public int MessageParameterAmount { get; private set; }
        
    }
    
    [CreateAssetMenu(fileName = "Quest", menuName = "Paws_parcels/Quest")]
    public class Quest : ScriptableObject
    {
        [field: SerializeField] public string QuestId { get; private set; }
        [field: SerializeField] public string QuestNameCode { get; private set; }
        [field: SerializeField, AllowNesting] public List<Mission> Missions { get; private set; }
        [field: SerializeField] public List<RewardReference> Rewards { get; private set; }
    }
    
    
    //не забыть про MessageSystem: с возможностью текстом подписываться на события. например вход в дом или открывание
    //двери. чтобы геймдиз сам это делал. как в проекте другом.
}