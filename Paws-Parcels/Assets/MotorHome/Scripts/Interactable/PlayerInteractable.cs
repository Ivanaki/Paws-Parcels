using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using ObservableCollections;

namespace MotorHome.Scripts
{
    public class PlayerInteractable : IInitializable, IDisposable
    {
        private LayerMask _layerMask;
        private CarPlayerSwitcher _carPlayerSwitcher;
        private PlayerPickUpper _playerPickUpper;
        
        private CoreUI _coreUI;
        private Transform _playerTransform;
        private InputAction _interact;
        private CompositeDisposable _disposable;
        private MessageGame _messageGame;
        private SoloMonolog _soloMonolog;
        private Dialog _dialog;
        
        private ObservableList<InteractableObject>  _availableInteractables = new();
        
        public PlayerInteractable(CoreUI coreUI, PlayerPickUpper playerPickUpper, CarPlayerSwitcher carPlayerSwitcher,
            MessageGame messageGame, SoloMonolog soloMonolog, Dialog dialog)
        {
            _coreUI = coreUI;
            _playerTransform = playerPickUpper.transform;
            _carPlayerSwitcher = carPlayerSwitcher;
            _playerPickUpper = playerPickUpper;
            _messageGame = messageGame;
            _soloMonolog = soloMonolog;
            _dialog = dialog;
        }
        
        public void Initialize()
        {
            _layerMask = LayerMask.GetMask("Player");
            
            _interact = InputSystem.actions.FindAction(ActionMaps.AllState + "/Interact");
            _interact.performed += Interact;

            _disposable = new CompositeDisposable();
            IDisposable dis = null;

            var collectionChanged = _availableInteractables.ObserveChanged().Select(_ => Unit.Default);
            var canGetInCar = _carPlayerSwitcher.CanGetInCar.Select(_ => Unit.Default);
            var canTakeObject = _playerPickUpper.CanTakeObject.Select(_ => Unit.Default);

            var merged = collectionChanged.Merge(canGetInCar).Merge(canTakeObject);

            dis = merged.Subscribe(_ => _coreUI.CanInteract(_availableInteractables.Count > 0 || 
                                                            _carPlayerSwitcher.CanGetInCar.CurrentValue ||
                                                            _playerPickUpper.CanTakeObject.CurrentValue));
            _disposable.Add(dis);
        }
        
        private void Interact(InputAction.CallbackContext obj)
        {
            if (_soloMonolog.IsStarted)
            {
                _soloMonolog.Click();
                return;
            }
            
            if (_dialog.IsStarted)
            {
                _dialog.Click();
                return;
            }
            
            var did = false;
            
            if (_carPlayerSwitcher.CurrentState.Value == CarPlayerSwitcher.CarOrPlayerState.Car)
            {
                _carPlayerSwitcher.ExitCar();
                return;
            }
            
            if (_carPlayerSwitcher.CanGetInCar.CurrentValue)
            {
                if (!_playerPickUpper.IsPlayerWithBox)
                { 
                    did = _playerPickUpper.CanPlayerInteract();
                    if(did) return;
                    
                    did = _carPlayerSwitcher.CanInteract();
                    if(!did) Debug.LogError("Player cannot enter car");
                    return;
                }
                else
                {
                    _messageGame.DoMessage("Ты не можешь войти, пока у тебя в руках предмет");
                }
            }
            else
            {
                if (_playerPickUpper.IsPlayerWithBox == false)
                {
                    did = _playerPickUpper.CanPlayerInteract();
                    if(did) return;
                    
                    did = DoInteractableList();
                    if(did) return;
                }
                else
                {
                    did = DoInteractableList();
                    if(did) return;
                
                    did = _playerPickUpper.CanPlayerInteract();
                    if(did) return;
                }
            }
        }

        private InteractableObject FindMinimumDistanceObject()
        {
            InteractableObject interactable = null;
            var minDistance = float.MaxValue;
            foreach (var i in _availableInteractables)
            {
                var dis = Vector3.Distance(i.PosOverlapsTransform.position, _playerTransform.position);
                if (dis < minDistance)
                {
                    interactable = i;
                    minDistance = dis;
                }
            }

            return interactable;
        }

        private bool DoInteractableList()
        {
            var flag1 = false;
            
            InteractableObject minInteractable = null;
            while (true)
            {
                if(_availableInteractables.Count == 0)
                    break;
                
                minInteractable = FindMinimumDistanceObject();

                var hitColliders = Physics.OverlapSphere(minInteractable.PosOverlapsTransform.position,
                    minInteractable.Radius, _layerMask);

                var flag2 = false;
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.TryGetComponent(out CharacterController characterController))
                    {
                        flag2 = true;
                        break;
                    }
                }

                if (flag2)
                {
                    flag1 = true;
                    break;
                }
                
                TryDelInteractable(minInteractable);
            }
            
            Debug.Log("1");

            if (flag1)
            {
                minInteractable.Interact();
                return true;
                //объект для взаимодействия найден
            }
            else
            {
                return false;
            }
        }

        public void AddInteractable(InteractableObject interactableObject)
        {
            _availableInteractables.Add(interactableObject);
        }

        public void TryDelInteractable(InteractableObject interactableObject)
        {
            if(_availableInteractables.Contains(interactableObject))
                _availableInteractables.Remove(interactableObject);
        }
        
        
        
        
        
        
        public void Dispose()
        {
            _disposable?.Dispose();
            _interact.performed -= Interact;
        }
    }
}