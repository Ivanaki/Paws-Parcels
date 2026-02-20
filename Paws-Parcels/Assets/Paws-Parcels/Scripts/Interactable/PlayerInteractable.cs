using System;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Paws_Parcels.Scripts.Player
{
    public class PlayerInteractable : MonoBehaviour
    {
        [SerializeField] private float _interactRadius = 1.5f;
        [SerializeField] private LayerMask _interactLayerMask;
        [SerializeField] private Transform _interactCenterOfSphere;

        private InputSystem_Actions _inputSystem;

        private Collider[] _hitColliders = new Collider[10];
        private HashSet<InteractableObject> _activatedInteractableObjects = new();
        private HashSet<InteractableObject> _newInteractableObjects = new();
        private InteractableObject _minDistanceInteractableObject = null;
        
        [Inject]
        private void Construct(GameInputService gameInputService)
        {
            _inputSystem = gameInputService.InputSystem;
        }
        
        private void OnEnable()
        {
            _inputSystem.PlayerNotMovement.Interact.performed += Interact;
        }
        
        private void OnDisable()
        {
            _inputSystem.PlayerNotMovement.Interact.performed -= Interact;
            DisableAllInteractablesAndClear(_activatedInteractableObjects);
            _minDistanceInteractableObject?.CouldntInteractWithMinDistance();
            _minDistanceInteractableObject = null;
        }

        private void FixedUpdate()
        {
            GetAvailableInteractables();
            var interactObj = FindMinimumDistanceObject();
            if (_minDistanceInteractableObject != interactObj)
            {
                _minDistanceInteractableObject?.CouldntInteractWithMinDistance();
                _minDistanceInteractableObject = interactObj;
                _minDistanceInteractableObject?.CanInteractWithMinDistance();
            }
        }
        
        private void Interact(InputAction.CallbackContext obj)
        {
            _minDistanceInteractableObject?.Interact();
        }

        private InteractableObject FindMinimumDistanceObject()
        {
            InteractableObject interactable = null;
            var minDistance = float.MaxValue;
            foreach (var interactableObj in _activatedInteractableObjects)
            {
                var dis = Vector3.Distance(interactableObj.transform.position, _interactCenterOfSphere.position);
                if (dis < minDistance)
                {
                    interactable = interactableObj;
                    minDistance = dis;
                }
            }

            return interactable;
        }
        
        private void GetAvailableInteractables()
        {
            //собираем все InteractableObject, которые находятся в нужном радиусе от плеера
            _newInteractableObjects.Clear();
            var size = Physics.OverlapSphereNonAlloc(_interactCenterOfSphere.position, _interactRadius, _hitColliders, _interactLayerMask);
            for (var i = 0; i < size; i++)
            {
                if(ColliderRedirector.TryGet(_hitColliders[i].gameObject, out InteractableObject interactableObject))
                {
                    _newInteractableObjects.Add(interactableObject);
                }
            }
            
            //отключаем все InteractableObject, которых уже нет в новой проверке.
            //ExceptWith - оставляет только объекты, которые есть в _interactableObjects, но уже нет в _newInteractableObjects
            _activatedInteractableObjects.ExceptWith(_newInteractableObjects); 
            foreach (var interactableObject in _activatedInteractableObjects)
            {
                interactableObject.CouldntInteract();
            }
            
            //активируем все, которые доступны
            foreach (var newInteractableObject in _newInteractableObjects)
            {
                newInteractableObject.CanInteract();
            }

            //меняем местами ссылки, чтобы не выделять память и лишний раз их не копировать
            (_activatedInteractableObjects, _newInteractableObjects) = (_newInteractableObjects, _activatedInteractableObjects);
            //теперь в _activatedInteractableObjects актуальные доступные элементы
        }
        
        private void DisableAllInteractablesAndClear(HashSet<InteractableObject> interactableObjects)
        {
            foreach (var interactableObject in interactableObjects)
            {
                interactableObject.CouldntInteract();
            }
            interactableObjects.Clear();
        }
    }
}