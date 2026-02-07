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

    [Serializable]
    public class NotSoloMessage
    {
        [SerializeField] private List<string> _type;
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
        [SerializeField] private string _textCode;
        
        [Header("Destination")]
        [SerializeField] private bool _hasDestination  = true;
        [SerializeField] private string _destinationCode;
        [SerializeField] private Vector3 _destinationPosition;
        
        [Header("Message")]
        [SerializeField] private bool _isSoloMessage  = true;
        
        [ShowIf(nameof(_isSoloMessage)), AllowNesting]
        [SerializeField] private string _message;
        
        [HideIf(nameof(_isSoloMessage)), AllowNesting] 
        [SerializeField] private NotSoloMessage _notSoloMessage;

        
    }
    
    [CreateAssetMenu(fileName = "Quest", menuName = "Paws_parcels/Quest")]
    public class Quest : ScriptableObject
    {
        public string _id;
        
        public bool ActiveOnAwake = true;
        [HideIf(nameof(ActiveOnAwake))] 
        public string _activateMessage;
        
        [Space]
        
        public string _giverNpcId;
        //Диалоги: берёшь квест, после взятия квеста короткая справка
        
        public List<Mission> _missions;
        public List<RewardReference> _rewards;
    }
    
    
    //не забыть про MessageSystem: с возможностью текстом подписываться на события. например вход в дом или открывание
    //двери. чтобы геймдиз сам это делал. как в проекте другом.
}