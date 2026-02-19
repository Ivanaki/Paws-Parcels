using Services;
using UnityEngine;
using Zenject;

namespace Quests.Message
{
    public class SendMessageFromGameObject : MonoBehaviour
    {
        [Inject] private MessageSystem _messageSystem;

        public void SetCompositeMessage(string message)
        {
            _messageSystem.SendCompositeMessage(this, message);
        }
    }
}