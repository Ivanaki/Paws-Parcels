using System;
using R3;

namespace Paws_Parcels.Scripts.Saves.States.GameState
{
    public class GameProxy
    {
        private readonly GameState _gameState;
        
        public Guid SaveId => _gameState.SaveId;
        public ReactiveProperty<int> TrackedMissionID { get; }
        public ReactiveProperty<bool> IsCompassActive { get; }
        
        public GameProxy(GameState gameState)
        {
            _gameState = gameState;
            
            TrackedMissionID = new ReactiveProperty<int>(gameState.TrackedMissionID);
            IsCompassActive = new ReactiveProperty<bool>(gameState.IsCompassActive);
            
            TrackedMissionID.Skip(1).Subscribe(v => gameState.TrackedMissionID = v);
            IsCompassActive.Skip(1).Subscribe(v => gameState.IsCompassActive = v);
        }

        public void Change(GameState gameState)
        {
            TrackedMissionID.Value = gameState.TrackedMissionID;
        }
        
        public void Change(GameProxy gameProxy)
        {
            TrackedMissionID.Value = gameProxy.TrackedMissionID.Value;
        }
        
        public int GetNewSavableEntityID()
        {
            return _gameState.GetNewSavableEntityID();
        }
        
        public int GetNewNonSavableEntityID()
        {
            return _gameState.GetNewNonSavableEntityID();
        }
    }
}