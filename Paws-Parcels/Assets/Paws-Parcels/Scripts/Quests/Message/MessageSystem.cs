using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Quests;
using Quests.Message;
using R3;
using UnityEngine;

namespace Services
{
    public class MessageSystem
    {
        public class ListenerInfo
        {
            public IMessageHandler listener;
            public string message;
            public string parameter;
            public int frameAdded;
            public bool removed;

            public ListenerInfo() { }
            
            public ListenerInfo(IMessageHandler listener, string message, string parameter)
            {
                this.listener = listener;
                this.message = message;
                this.parameter = parameter;
                this.frameAdded = Time.frameCount;
                this.removed = false;
            }

            public void Assign(IMessageHandler listener, string message, string parameter)
            {
                this.listener = listener;
                this.message = message;
                this.parameter = parameter;
                this.frameAdded = Time.frameCount;
                this.removed = false;
            }

            public void Clear()
            {
                this.listener = null;
                this.message = null;
                this.parameter = null;
                this.removed = false;
            }
        }
        
        [Serializable]
        private class MessageObject
        {
            public string Message;
            public int InvokeCount = 0;
            public Dictionary<string, ParameterObject> ParametersDictionary = new();
            
            [JsonIgnore]
            public List<ListenerInfo> ListenerInfos = new();
        }
        
        [Serializable]
        private class ParameterObject
        {
            public int InvokeCount = 0;
            public string Parameter;
            
            [JsonIgnore]
            public List<ListenerInfo> ListenerInfos = new();
        }

        
        private readonly bool allowReceiveSameFrameAdded = false;
        private readonly bool debug = true;
        private readonly bool _allowExceptions = false;
        
        private readonly HashSet<GameObject> _sendersToLog = new();
        private readonly HashSet<GameObject> _listenersToLog = new();
        private readonly Pool<ListenerInfo> _listenerInfoPool = new();
        
        private Dictionary<string, MessageObject> _messagesDictionary = new();
        //private List<ListenerInfo> _allMessages = new();

        private int _sendMessageDepth = 0;

        public class Subscription : IDisposable, IMessageHandler
        {
            private readonly string _comboMessage;
            private readonly MessageSystem _messageSystem;
            private Subject<MessageArgs> _subject = new();

            public Subscription(string comboMessage, MessageSystem messageSystem)
            {
                _comboMessage = comboMessage;
                _messageSystem = messageSystem;
            }
            
            public IDisposable Subscribe(Action<MessageArgs> observer)
            {
                _subject.Subscribe(observer);
                return this;
            }
            
            public void Dispose()
            {
                _subject.Dispose();
                _messageSystem.RemoveListener(this, _comboMessage);
            }

            public void OnMessage(MessageArgs messageArgs)
            {
                _subject.OnNext(messageArgs);
            }
        }
        
        public Subscription AddListener(string comboMessage)
        {
            var s = new Subscription(comboMessage, this);
            AddListener(s, comboMessage);
            return s;
        }
        
        public void AddListener(IMessageHandler listener, string comboMessage)
        {
            var par = GetParameters(comboMessage);
            
            AddListener(listener, par.Item1, par.Item2);
        }
        
        public void AddListener(IMessageHandler listener, string message, string parameter)
        {
            if (string.IsNullOrEmpty(message))
            {
                //_allMessages.Add(info);
                return;
            }
            
            if (debug) Debug.Log("MessageSystem.AddListener(listener=" + listener + ": " + message + "," + parameter + ")");

            if (string.IsNullOrEmpty(message) && string.IsNullOrEmpty(parameter) == false)
            {
                Debug.LogError(@"При пустом message parameter не может быть пустым. Parameter заменён на """". Этот подписчик подписался на все вызовы");
                parameter = "";
            }

            // Check if listener is already registered:
            var allListeners = GetAllListenerLists();
            foreach (var listenerInfos in allListeners)
            {
                foreach (var x in listenerInfos)
                {
                    if (x.listener == listener && string.Equals(x.message, message) && (string.Equals(x.parameter, parameter) || string.IsNullOrEmpty(x.parameter)))
                    {
                        x.removed = false;
                        return;
                    }
                }
            }
            

            // Otherwise add:
            var info = _listenerInfoPool.Get();
            info.Assign(listener, message, parameter);

            if (string.IsNullOrEmpty(message))
            {
                //_allMessages.Add(info);
                return;
            }
            
            if (_messagesDictionary.ContainsKey(message) == false)
                _messagesDictionary.Add(message, new MessageObject{Message = message});

            var messageObject = _messagesDictionary[message];
            if (string.IsNullOrEmpty(parameter))
            {
                messageObject.ListenerInfos.Add(info);
                return;
            }
            
            if (messageObject.ParametersDictionary.ContainsKey(parameter) == false)
                messageObject.ParametersDictionary.Add(parameter, new ParameterObject() {Parameter = parameter});
                
            messageObject.ParametersDictionary[parameter].ListenerInfos.Add(info);
        }
        
        public void RemoveListener(IMessageHandler listener, string comboMessage)
        {
            var par = GetParameters(comboMessage);
            
            RemoveListener(listener, par.Item1, par.Item2);
        }
        
        public void RemoveListener(IMessageHandler listener, string message, string parameter)
        {
            if (debug) Debug.Log("MessageSystem.RemoveListener(listener=" + listener + ": " + message + "," + parameter + ")");
            
            if (string.IsNullOrEmpty(message))
            {
                if (string.IsNullOrEmpty(parameter))
                    Debug.LogError("При пустом message parameter не может быть пустым. Этот подписчик отписался от всех вызовов");
                //Remove(_allMessages);
                return;
            }
            
            if(_messagesDictionary.TryGetValue(message, out var messageObject))
            {
                if (string.IsNullOrEmpty(parameter))
                {
                    Remove(messageObject.ListenerInfos);
                    return;
                }

                if (messageObject.ParametersDictionary.TryGetValue(parameter, out var parameterObject))
                {
                    Remove(parameterObject.ListenerInfos);
                    return;
                }
            }
            
            return;
            
            void Remove(List<ListenerInfo> listenerInfo)
            {
                if (listenerInfo.Count <= 0) return;
                for (int i = listenerInfo.Count - 1; i >= 0; i--)
                {
                    var x = listenerInfo[i];
                    if (x.listener == listener &&
                        (string.Equals(x.message, message) || string.IsNullOrEmpty(message)) &&
                        (string.Equals(x.parameter, parameter) || string.IsNullOrEmpty(parameter)))
                    {
                        x.removed = true;
                        if (_sendMessageDepth == 0)
                        {
                            listenerInfo.RemoveAt(i);
                            x.Clear();
                            _listenerInfoPool.Release(x);
                        }
                    }
                }
            }
        }

        public void SendMessageWithTarget(object sender, object target, string message, string parameter, params object[] values)
        {
            if (Application.isPlaying == false) return;
            if (debug || ShouldLogSender(sender)) Debug.Log("MessageSystem.SendMessage(sender=" + sender +
                ((target == null) ? string.Empty : (" target=" + target)) +
                ": " + message + "," + parameter + ")");
            var messageArgs = new MessageArgs(sender, target, message, parameter, values); // struct passed on stack; no heap allocated.
            try
            {
                _sendMessageDepth++;
                
                //ApplyListeners(_allMessages, messageArgs);

                if (string.IsNullOrEmpty(message) == false)
                {
                    if (_messagesDictionary.TryGetValue(message, out var messageObject))
                    {
                        messageObject.InvokeCount++;
                        ApplyListeners(messageObject.ListenerInfos, messageArgs);
                    }
                    else
                    {
                        messageObject = new MessageObject { InvokeCount = 1, Message = message };
                        _messagesDictionary.Add(message, messageObject);
                    }

                    if (string.IsNullOrEmpty(parameter) == false)
                    {
                        if (messageObject.ParametersDictionary.TryGetValue(parameter, out var parameterObject))
                        {
                            parameterObject.InvokeCount++;
                            ApplyListeners(parameterObject.ListenerInfos, messageArgs);
                        }
                        else
                        {
                            messageObject.ParametersDictionary.Add(parameter, new ParameterObject(){InvokeCount = 1, Parameter = parameter });
                        }
                    }
                }
            }
            finally
            {
                _sendMessageDepth--;
                if (_sendMessageDepth == 0)
                {
                    var listeners = GetAllListenerLists();
                    foreach (var listenerInfos in listeners)
                    {
                        RemoveMarkedListenerInfo(listenerInfos);
                    }
                }
            }
        }
        
        private void RemoveMarkedListenerInfo(List<ListenerInfo> listeners)
        {
            var listenersToRemove = listeners.FindAll(x => x.removed);
            listeners.RemoveAll(x => x.removed);
            foreach (var listenerToRemove in listenersToRemove)
            {
                listenerToRemove.Clear();
                _listenerInfoPool.Release(listenerToRemove);
            }
        }

        private void ApplyListeners(List<ListenerInfo> listeners, MessageArgs messageArgs)
        {
            foreach (var x in listeners)
            {
                if (x == null || x.removed) continue;
                if (x.listener == null)
                {
                    x.removed = true;
                    continue;
                }

                if (allowReceiveSameFrameAdded == false && x.frameAdded == Time.frameCount)
                {
                    Debug.LogWarning("Listener: " + x.listener + " subscribed in one frame with message sent");
                    continue;
                }
                if (string.Equals(x.message, messageArgs.message) &&
                    (string.Equals(x.parameter, messageArgs.parameter) || string.IsNullOrEmpty(x.parameter))
                    || string.IsNullOrEmpty(x.message))
                {
                    if (_allowExceptions && Debug.isDebugBuild)
                    {
                        SendMessageToListener(x, messageArgs);
                    }
                    else
                    {
                        try
                        {
                            SendMessageToListener(x, messageArgs);
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError("Message System exception sending '" + messageArgs.message + "'/'" +
                                           messageArgs.parameter + "' to " + x.listener + ": " + e.Message);
                        }
                    }
                }
            }
        }
        
        private void SendMessageToListener(ListenerInfo x, MessageArgs messageArgs)
        {
            if (ShouldLogReceiver(x.listener))
            {
                Debug.Log("MessageSystem.SendMessage(sender=" + messageArgs.sender +
                          ((messageArgs.target == null) ? string.Empty : (" target=" + messageArgs.target)) +
                          ": " + messageArgs.message + "," + messageArgs.parameter + ")");
            }
            x.listener.OnMessage(messageArgs);
        }
        
        /// <summary>
        /// Log a debug message when this object sends a message.
        /// </summary>
        public void LogWhenSendingMessages(GameObject sender)
        {
            if (sender == null) return;
            _sendersToLog.Add(sender);
        }

        /// <summary>
        /// Stop logging debug messages when this object sends a message.
        /// </summary>
        public void StopLoggingWhenSendingMessages(GameObject sender)
        {
            if (sender == null) return;
            _sendersToLog.Remove(sender);
        }

        /// <summary>
        /// Log a debug message when this listener receives a message.
        /// </summary>
        public void LogWhenReceivingMessages(GameObject listener)
        {
            if (listener == null) return;
            _listenersToLog.Add(listener);
        }

        /// <summary>
        /// Stop logging debug messages when this listener receives a message.
        /// </summary>
        public void StopLoggingWhenReceivingMessages(GameObject listener)
        {
            if (listener == null) return;
            _listenersToLog.Add(listener);
        }
        
        private bool ShouldLogSender(object sender)
        {
            if (sender is UnityEngine.Object o && o == null) return false;
            return (sender is GameObject gameObject && _sendersToLog.Contains(gameObject)) ||
                   (sender is Component component && _sendersToLog.Contains(component.gameObject));
        }

        private bool ShouldLogReceiver(IMessageHandler receiver)
        {
            return (receiver is Component component && component != null && _listenersToLog.Contains(component.gameObject));
        }
        
        public bool IsListenerRegistered(IMessageHandler listener, string message, string parameter)
        {
            return GetAllListenerInfos().Any(Check);
            
            bool Check(ListenerInfo x) =>
                !x.removed &&
                x.listener == listener &&
                x.message == message &&
                (x.parameter == parameter || string.IsNullOrEmpty(x.parameter));
        }
        
        public void SendCompositeMessage(object sender, string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            var parameter = string.Empty;
            object value = null;
            if (message.Contains(":")) // Parameter?
            {
                var colonPos = message.IndexOf(':');
                parameter = message.Substring(colonPos + 1);
                message = message.Substring(0, colonPos);

                if (parameter.Contains(":")) // Value?
                {
                    colonPos = parameter.IndexOf(':');
                    var valueString = parameter.Substring(colonPos + 1);
                    parameter = parameter.Substring(0, colonPos);
                    int valueInt;
                    bool isNumeric = int.TryParse(valueString, out valueInt);
                    if (isNumeric) value = valueInt; else value = valueString;
                }
            }
            if (value == null)
            {
                SendMessageWithTarget(sender, null, message, parameter);
            }
            else
            {
                SendMessageWithTarget(sender, null, message, parameter, value);
            }
        }
        
        private IEnumerable<ListenerInfo> GetAllListenerInfos()
        {
            /*foreach (var info in _allMessages)
                yield return info;*/

            foreach (var messageObject in _messagesDictionary.Values)
            {
                foreach (var info in messageObject.ListenerInfos)
                    yield return info;

                foreach (var parameterObject in messageObject.ParametersDictionary.Values)
                foreach (var info in parameterObject.ListenerInfos)
                    yield return info;
            }
        }

        private IEnumerable<List<ListenerInfo>> GetAllListenerLists()
        {
            //yield return _allMessages;

            foreach (var messageObject in _messagesDictionary.Values)
            {
                yield return messageObject.ListenerInfos;

                foreach (var parameterObject in messageObject.ParametersDictionary.Values)
                    yield return parameterObject.ListenerInfos;
            }
        }

        private (string, string, object) GetParameters(string compositeMessage)
        {
            if (string.IsNullOrEmpty(compositeMessage)) return ("","", null);
            var message = compositeMessage;
            var parameter = string.Empty;
            object value = null;
            if (message.Contains(":")) // Parameter?
            {
                var colonPos = message.IndexOf(':');
                parameter = message.Substring(colonPos + 1);
                message = message.Substring(0, colonPos);

                if (parameter.Contains(":")) // Value?
                {
                    colonPos = parameter.IndexOf(':');
                    var valueString = parameter.Substring(colonPos + 1);
                    parameter = parameter.Substring(0, colonPos);
                    int valueInt;
                    bool isNumeric = int.TryParse(valueString, out valueInt);
                    if (isNumeric) value = valueInt; else value = valueString;
                }
            }

            return (message, parameter, value);
        }
    }
}