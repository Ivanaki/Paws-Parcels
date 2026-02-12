using System;
using UnityEngine;
using Zenject;

namespace Quests
{
    public class MarkObject : MonoBehaviour
    {
        public string Id => _id;
        
        [SerializeField] private string _id;
        [SerializeField] private GameObject _showableObject;
        [SerializeField] private Behaviour _showableComponent;
        

        [Inject] private DestinationService _destinationService;

        private void Awake()
        {
            Hide();
        }

        private void OnEnable()
        {
            _destinationService.AddMark(this);
        }

        private void OnDisable()
        {
            _destinationService.RemoveMark(this);
        }

        public void Show()
        {
            if(_showableObject != null)
                _showableObject?.SetActive(true);
            if(_showableComponent != null)
                _showableComponent.enabled = true;
        }

        public void Hide()
        {
            if(_showableObject != null)
                _showableObject?.SetActive(false);
            if(_showableComponent != null)
                _showableComponent.enabled = false;
        }
    }
}