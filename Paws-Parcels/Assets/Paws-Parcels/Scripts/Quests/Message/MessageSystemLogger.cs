
using Services;
using UnityEngine;
using Zenject;

namespace Messages
{
    public class MessageSystemLogger : MonoBehaviour
    {

        [Tooltip("Log a message when this GameObject sends a message to the Message System.")]
        public bool logWhenSendingMessages;

        [Tooltip("Log a message when this GameObject receives a message from the Message System.")]
        public bool logWhenReceivingMessages;

        [Inject] private MessageSystem _messageSystem;
        
        private void OnEnable()
        {
            if (logWhenSendingMessages)
            {
                _messageSystem.LogWhenSendingMessages(gameObject);
            }
            if (logWhenReceivingMessages)
            {
                if (GetComponent(typeof(IMessageHandler)) == null)
                {
                    if (Debug.isDebugBuild) Debug.LogWarning("MessageSystem: " + name + " doesn't have any IMessageHandler components. Can't log when receiving messages.", this);
                }
                else
                {
                    _messageSystem.LogWhenReceivingMessages(gameObject);
                }
            }
        }

        private void OnDisable()
        {
            if (logWhenSendingMessages)
            {
                _messageSystem.StopLoggingWhenSendingMessages(gameObject);
            }
            if (logWhenReceivingMessages)
            {
                _messageSystem.StopLoggingWhenReceivingMessages(gameObject);
            }
        }
    }
}