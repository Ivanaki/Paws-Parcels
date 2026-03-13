using System;
using Paws_Parcels.Extentions;
using Paws_Parcels.Scripts.Player;
using R3;
using UnityEngine;
using Zenject;

namespace Car_Player
{
    [RequireComponent(typeof(ThirdPersonController))]
    [RequireComponent(typeof(CharacterController))]
    public class PutCarUpTeleporter : MonoDisposable
    {
        [SerializeField] private LayerMask _rayLayerMask;
        [SerializeField] private float _maxDistanceRay = 5f;
        //[SerializeField] private float _timeToMove = 1.5f;
        
        [Inject] private PutInCarItemsService _putInCarItemsService;
        [Inject] private ApplyInFixUpdate _applyInFixUpdate;
        
        private Transform _playerTeleportTransformWhenPut;
        
        private void Awake()
        {
            _playerTeleportTransformWhenPut = GameObject.FindGameObjectWithTag("PutObjectPlayerTeleportPoint").transform;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _disposable.Add(_putInCarItemsService.PutState.Subscribe(state =>
            {
                if (state == PutItemState.Put)
                {
                    if(_playerTeleportTransformWhenPut == null) Awake();
                    if (_playerTeleportTransformWhenPut != null)
                    {
                        if (Physics.Raycast(_playerTeleportTransformWhenPut.position, Vector3.down,
                                out var hit, _maxDistanceRay, _rayLayerMask))
                        {
                            _applyInFixUpdate.ObserveApply().Take(1).Subscribe(_ =>
                            {
                                transform.position = hit.point;
                            });
                        }
                        else
                        {
                            _applyInFixUpdate.ObserveApply().Take(1).Subscribe(_ =>
                            {
                                transform.position = _playerTeleportTransformWhenPut.position;
                            });
                        }
                    }
                }
            }));
        }
    }
}