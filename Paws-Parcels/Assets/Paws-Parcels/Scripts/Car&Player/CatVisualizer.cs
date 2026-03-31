using Paws_Parcels.Scripts;
using R3;
using UnityEngine;

namespace Car_Player
{
    public class CatVisualizer : BaseVisualizer
    {
        [SerializeField] private GameObject _catGameObject;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _gameProxy.IsCatInCar.Subscribe(v =>
            {
                _catGameObject.SetActive(v);
            });
        }
    }
}