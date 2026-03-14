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

        public Observable<Collider> OnTriggerEnterObservable => _onTriggerEnterSubject;
        public Observable<Collider> OnTriggerExitObservable => _onTriggerExitSubject;
        
        private readonly Subject<Collider> _onTriggerEnterSubject = new();
        private readonly Subject<Collider> _onTriggerExitSubject = new();

        protected virtual void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }
        
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (_checkTag == false || (_checkTag && CheckTags(other.tag)))
            {
                _onTriggerEnterUnityEvent.Invoke(other.gameObject);
                _onTriggerEnterSubject.OnNext(other);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (_checkTag == false || (_checkTag && CheckTags(other.tag)))
            {
                _onTriggerExitUnityEvent.Invoke(other.gameObject);
                _onTriggerExitSubject.OnNext(other);
            }
        }

        private bool CheckTags(string tag) => tag == _tag;
    }
}