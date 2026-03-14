using System;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Zenject;

namespace Paws_Parcels.Scripts.Player
{
    public class PlayerShowableService
    {
        private readonly GameStateProvider _gameStateProvider;
        public ReadOnlyReactiveProperty<bool> ShowPlayer => _showPlayer;
        
        private readonly ReactiveProperty<bool> _showPlayer = new(true);

        private bool _show = true;

        public PlayerShowableService(GameStateProvider gameStateProvider)
        {
            _gameStateProvider = gameStateProvider;
            gameStateProvider.GameProxy.IsPlayerInCar.Subscribe(value => UpdateState());
            UpdateState();
        }
        
        public void Hide()
        {
            _show = false;
            UpdateState();
        }

        public void Show()
        {
            _show = true;
            UpdateState();
        }
        
        private void UpdateState()
        {
            if (_gameStateProvider.GameProxy.IsPlayerInCar.Value)
            {
                _showPlayer.Value = false;
            }
            else
            {
                _showPlayer.Value = _show;
            }
        }
    }
}