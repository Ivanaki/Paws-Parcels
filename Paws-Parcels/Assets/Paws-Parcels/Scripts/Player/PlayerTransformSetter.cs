using System;
using Paws_Parcels.Scripts.Player;
using Services;
using UnityEngine;
using Zenject;

namespace Player
{
    [RequireComponent(typeof(ThirdPersonController))]
    public class PlayerTransformSetter : MonoBehaviour
    {
        [Inject] private PlayerService _playerService;

        private void Awake()
        {
            _playerService.SetPlayerTransform(transform);
        }

        private void OnEnable()
        {
            _playerService.SetPlayerTransform(transform);
        }
    }
}