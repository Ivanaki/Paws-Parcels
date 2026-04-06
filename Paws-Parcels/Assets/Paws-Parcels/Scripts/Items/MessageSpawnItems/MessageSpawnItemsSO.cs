using System.Collections.Generic;
using Paws_Parcels.Scripts.Saves.States.GameState;
using UnityEngine;

namespace Items.MessageSpawnItems
{
    [System.Serializable]
    public class ItemDataWithMessage
    {
        public string Message;
        public ItemData ItemData;
    }
    
    [CreateAssetMenu(fileName = "MessageSpawnItems", menuName = "Paws_parcels/Message Spawn Items")]
    public class MessageSpawnItemsSO : ScriptableObject
    {
        public List<ItemDataWithMessage> ItemDataWithMessages;
    }
}