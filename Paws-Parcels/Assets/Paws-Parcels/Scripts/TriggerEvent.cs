using System;
using System.Collections.Generic;
using NaughtyAttributes;
using R3;
using UnityEngine;
using UnityEngine.Events;

namespace Paws_Parcels.Scripts
{
    [RequireComponent(typeof(Collider))]
    public class TriggerEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent<GameObject> _onTriggerEnterUnityEvent = new();
        [SerializeField] private UnityEvent<GameObject> _onTriggerExitUnityEvent = new();

        [SerializeField] private bool _checkTag;
        
        [ShowIf(nameof(_checkTag)), Tag]
        [SerializeField] private string _tag;
        
        public Observable<GameObject> OnTriggerEnterObservable {get; private set;}
        public Observable<GameObject> OnTriggerExitObservable {get; private set;}

        protected virtual void Awake()
        {
            OnTriggerEnterObservable = _onTriggerEnterUnityEvent.AsObservable();
            OnTriggerExitObservable = _onTriggerExitUnityEvent.AsObservable();
            
            GetComponent<Collider>().isTrigger = true;
        }
        
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (_checkTag == false || (_checkTag && CheckTags(other.tag)))
            {
                _onTriggerEnterUnityEvent.Invoke(other.gameObject);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (_checkTag == false || (_checkTag && CheckTags(other.tag)))
            {
                _onTriggerExitUnityEvent.Invoke(other.gameObject);
            }
        }

        private bool CheckTags(string tag) => tag == _tag;
    }
}