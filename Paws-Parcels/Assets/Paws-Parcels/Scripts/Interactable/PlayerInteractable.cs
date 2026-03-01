using System.Collections.Generic;
using Paws_Parcels.Extentions;
using Services;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Paws_Parcels.Scripts.Player
{
    public class PlayerInteractable : MonoBehaviour
    {
        [SerializeField] private float _interactRadius = 1.5f;
        [SerializeField] private float _highlightRadius = 2.5f;
        [SerializeField] private LayerMask _interactLayerMask;
        [SerializeField] private Transform _interactCenterOfSphere;

        private InputSystem_Actions _inputSystem;

        
        
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
            DisableAllInteractablesAndClear(_lastInteractableObjects);
            _minDistanceInteractableObject?.CouldntInteractWithMinDistance();
            _minDistanceInteractableObject = null;
        }

        private void FixedUpdate()
        {
            HighlightInteractables();
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

        private Collider[] _hitCollidersInteract = new Collider[20];
        private readonly List<InteractableObject> _interactables = new();
        
        private InteractableObject FindMinimumDistanceObject()
        {
            _interactables.Clear();
            var size = Physics.OverlapSphereNonAlloc(_interactCenterOfSphere.position, _interactRadius, _hitCollidersInteract, _interactLayerMask);
            for (var i = 0; i < size; i++)
            {
                if(ColliderRedirector.TryGet(_hitCollidersInteract[i].gameObject, out InteractableObject interactableObject))
                {
                    _interactables.Add(interactableObject);
                }
            }
            
            InteractableObject interactable = null;
            var minDistance = float.MaxValue;
            foreach (var interactableObj in _interactables)
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
        
        private HashSet<InteractableObject> _lastInteractableObjects = new();
        private HashSet<InteractableObject> _newInteractableObjects = new();
        private readonly HashSet<InteractableObject> _temp = new();
        private readonly Collider[] _hitCollidersHighlight = new Collider[20];
        
        private void HighlightInteractables()
        {
            //собираем все InteractableObject, которые находятся в нужном радиусе от плеера
            _newInteractableObjects.Clear();
            var size = Physics.OverlapSphereNonAlloc(_interactCenterOfSphere.position, _highlightRadius, _hitCollidersHighlight, _interactLayerMask);
            for (var i = 0; i < size; i++)
            {
                if(ColliderRedirector.TryGet(_hitCollidersHighlight[i].gameObject, out InteractableObject interactableObject))
                {
                    _newInteractableObjects.Add(interactableObject);
                }
            }
            
            //м_1.ExceptWith(м_2) - операция вычитания в множествах: м_1 = м_1 - м_2
            
            //получаем новые, которых не было в прошлой проверке
            _temp.Clear();
            _temp.UnionWith(_newInteractableObjects); //в данном случае копируем все элементы в temp
            _temp.ExceptWith(_lastInteractableObjects); 
            foreach (var interactableObject in _temp)
                interactableObject.Highlight();
            
            
            //получаем старые, которых уже нет в новой проверке
            _temp.Clear();
            _temp.UnionWith(_lastInteractableObjects); //в данном случае копируем все элементы в temp
            _temp.ExceptWith(_newInteractableObjects); 
            foreach (var interactableObject in _temp)
                interactableObject.Unhighlight();
            
            //меняем местами ссылки, чтобы не выделять память и лишний раз их не копировать
            (_lastInteractableObjects, _newInteractableObjects) = (_newInteractableObjects, _lastInteractableObjects);
            //теперь в _activatedInteractableObjects актуальные доступные элементы
        }
        
        private void DisableAllInteractablesAndClear(HashSet<InteractableObject> interactableObjects)
        {
            foreach (var interactableObject in interactableObjects)
            {
                interactableObject.Unhighlight();
            }
            interactableObjects.Clear();
        }
    }
}