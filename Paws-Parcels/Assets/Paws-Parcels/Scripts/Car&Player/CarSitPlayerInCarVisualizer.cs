using Paws_Parcels.Scripts;
using R3;
using UnityEngine;

namespace Paws_Parcels.Car
{
    public class CarSitPlayerInCarVisualizer : BaseVisualizer
    {
        [SerializeField] private GameObject _playerManiken;
        //[SerializeField] private Animator _doorOpenAnimator;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _disposable.Add(_gameProxy.IsPlayerInCar.Subscribe(value =>
            {
                _playerManiken.SetActive(value);
                //_doorOpenAnimator.
            }));
        }
    }
}