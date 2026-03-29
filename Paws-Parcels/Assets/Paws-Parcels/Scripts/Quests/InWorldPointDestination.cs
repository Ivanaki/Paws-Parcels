    using R3;
    using Services;
    using UnityEngine;
    using Zenject;

    namespace Quests
    {
        public class InWorldPointDestination : DestinationObject
        {
            public override string DestinationId => _destinationId;
            public override string UiShowTypeId => _uiShowTypeId;
            public override ReadOnlyReactiveProperty<bool> IsDestinationShow => _isDestinationShow;

            protected override float ActivationDistance => _activationDistance;
            protected override float SelectedActivationDistance => _selectedActivationDistance;
            
            [Header("InWorldPointDestination Settings")]
            [SerializeField] private string _destinationId;
            [SerializeField] private string _uiShowTypeId;
            [SerializeField] private float _activationDistance = 10f;
            [SerializeField] private float _selectedActivationDistance = 20f;
            
           
            
            private readonly ReactiveProperty<bool> _isDestinationShow = new(false);

            [Inject] private PlayerService _playerService;
            
            private Vector3 _desPos;
            private float _currentNeedDistance;
            
            private void FixedUpdate()
            {
                if (_playerService.PlayerTransform != null)
                {
                    _desPos = (_destinationShowUiPosition != null
                        ? _destinationShowUiPosition.position
                        : transform.position);
                    _currentNeedDistance =
                        IsQuestSelect.CurrentValue ? SelectedActivationDistance : ActivationDistance;

                    if (Vector3.Distance(_desPos, _playerService.PlayerTransform.position)
                        <= _currentNeedDistance)
                    {
                        _isDestinationShow.Value = true;
                        return;
                    }
                }

                _isDestinationShow.Value = false;
            }
        }
    }