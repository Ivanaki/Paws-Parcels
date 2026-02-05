using R3;
using UnityEngine;
using UnityEngine.Events;

namespace Paws_Parcels.Scripts
{
    
    public class TriggerEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent<GameObject> _onTriggerEnterUnityEvent = new();
        [SerializeField] private UnityEvent<GameObject> _onTriggerExitUnityEvent = new();
        
        public Observable<GameObject> OnTriggerEnterObservable {get; private set;}
        public Observable<GameObject> OnTriggerExitObservable {get; private set;}

        protected virtual void Awake()
        {
            OnTriggerEnterObservable = _onTriggerEnterUnityEvent.AsObservable();
            OnTriggerExitObservable = _onTriggerExitUnityEvent.AsObservable();
        }
        
        protected virtual void OnTriggerEnter(Collider other)
        {
            _onTriggerEnterUnityEvent.Invoke(other.gameObject);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            _onTriggerExitUnityEvent.Invoke(other.gameObject);
        }
    }
}