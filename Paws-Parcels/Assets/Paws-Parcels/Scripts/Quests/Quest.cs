using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

namespace Paws_Parcels.Scripts.Quests
{
    /*public enum MessagesOperation
    {
        
    }*/
    
    [Serializable]
    public class RewardReference
    {
        public string Reward;
        public int Count;
    }

    [Serializable]
    public class ManyMessages
    {
        public List<MessageAsset> Messages;
    }
    
    [Serializable]
    public class ManyDestinations
    {
        public List<DestinationAsset> Destinations;
    }

    [Serializable]
    public class DestinationAsset
    {
        public string Id;
        public bool NotHighlightOnAwake;
        public string OffHighlightMessage;
        public string OnHighlightMessage;
    }
    
    [Serializable]
    public class MessageAsset
    {
        //public Guid Id;
        public string Message;
        public bool HasCount;
        [ShowIf(nameof(HasCount)), AllowNesting]
        public int MessageCount;
        [ShowIf(nameof(HasCount)), AllowNesting]
        public string MinusMessage;
    }
    
    /*[Serializable]
    public class BoolObject<T>
    {
        [field: SerializeField] public bool Active { get; private set; }
        [field: SerializeField] public T Value { get; private set; }
    }*/

    [Serializable]
    public class DestinationParent
    {
        //public bool SpecificDistance;
        //[ShowIf(nameof(SpecificDistance)), AllowNesting]
        //public float DestinationShowDistance = 5f;
        
        public bool IsMany;
        [HideIf(nameof(IsMany)), AllowNesting]
        public DestinationAsset Destination;
        [ShowIf(nameof(IsMany)), AllowNesting]
        public ManyDestinations ManyDestinations;
    }

    [Serializable]
    public class DialogueParent
    {
        public string NpcName;
        public DialogueReference MainDialogue;
        public DialogueReference AfterMainDialogue;
    }
    
    [Serializable]
    public class Mission
    {
        //public string Id;
        
        //public string ShortPhraseCode;
        //[TextArea(3,5)] public string DescriptionCode;
        
        [Header("Dialogue")]
        public bool IsThisMissionDialogue;
        [ShowIf(nameof(IsThisMissionDialogue)), AllowNesting]
        public DialogueParent DialogueParent;
        
        [Header("Destination")]
        public bool HasDestination;
        [ShowIf(nameof(HasDestination)), AllowNesting]
        public DestinationParent DestinationParent;
        
        
        [Header("Message to complete mission")]
        [HideInInspector]public bool IsMany;
        
        [HideIf(nameof(IsMany)), AllowNesting]
        public MessageAsset Message;
        
        [ShowIf(nameof(IsMany)), AllowNesting] 
        public ManyMessages ManyMessages;

        
    }
    
    [CreateAssetMenu(fileName = "Quest", menuName = "Paws_parcels/Quest")]
    public class Quest : ScriptableObject
    {
        public string Id;
        
        [Space]
        [HideInInspector]public bool ActiveOnAwake = true;
        /*[HideIf(nameof(ActiveOnAwake))] 
        public string ActivateMessage;*/
        
        
        //[Space]
        //public bool HasMonologBeforeQuestActivate = false; // [QuestName]:AwakeMonolog
        
        /*[Space]
        public bool HasGiver = false;
        [ShowIf(nameof(HasGiver))]
        public string GiverId; // Quest:GiverDialog*/
        
        //Диалоги: берёшь квест, после взятия квеста короткая справка
        
        [Space]
        public List<Mission> Missions;
        
        [Space]
        public List<RewardReference> Rewards;
        
        
        /*[Space]
        [ShowNonSerializedField] private string _localizationWords = "Name; ;;;;";*/
    }
    
    
    //не забыть про MessageSystem: с возможностью текстом подписываться на события. например вход в дом или открывание
    //двери. чтобы геймдиз сам это делал. как в проекте другом.
}