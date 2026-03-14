using Paws_Parcels.Extentions;
using R3;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(ThirdPersonController))]
    [RequireComponent(typeof(PlayerInteractable))]
    public class ShowPlayerVisualizer : MonoDisposable
    {
        [SerializeField] private GameObject _modelToHide;
        
        private ThirdPersonController _thirdPersonController;
        private CharacterController _characterController;
        private PlayerInteractable _playerInteractable;

        [Inject] private PlayerShowableService _playerShowableService;
        [Inject] private ApplyInFixUpdate _applyInFixUpdate;
        
        
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _thirdPersonController = GetComponent<ThirdPersonController>();
            _playerInteractable = GetComponent<PlayerInteractable>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _disposable.Add(_playerShowableService.ShowPlayer.Subscribe(value =>
            {
                _applyInFixUpdate.ObserveApply().Take(1).Subscribe(_ =>
                {
                    _thirdPersonController.enabled = value;
                    _characterController.enabled = value;
                    _playerInteractable.enabled = value;
                    _modelToHide.SetActive(value);
                });
            }));
        }
    }
}