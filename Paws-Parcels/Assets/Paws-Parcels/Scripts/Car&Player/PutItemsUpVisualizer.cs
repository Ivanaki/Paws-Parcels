using System;
using Paws_Parcels.Extentions;
using Paws_Parcels.Scripts;
using R3;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Car_Player
{
    public class PutItemsUpVisualizer : MonoDisposable
    {
        [SerializeField] private CinemachineCamera _upCamera;
        
        [Inject] private PutInCarItemsService _putInCarItemsService;

        private void Awake()
        {
            _upCamera.Priority.Value = -100;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _disposable.Add(_putInCarItemsService.PutState.Subscribe(state =>
            {
                if (state is PutItemState.Take or PutItemState.Put)
                {
                    _upCamera.Priority.Value = 100;
                }
                else
                {
                    _upCamera.Priority.Value = -100;
                }
            }));
        }
    }
}