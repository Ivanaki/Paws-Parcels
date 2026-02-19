using System;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class DestroyItemInHands : MonoBehaviour
    {
        [Inject]
        private PlayerPickUpper _playerPickUpper;
        
        public void Destroy()
        {
            Destroy(_playerPickUpper.GetCurrentItem().gameObject);
            _playerPickUpper.PutObjectToCar();
        }
    }
}