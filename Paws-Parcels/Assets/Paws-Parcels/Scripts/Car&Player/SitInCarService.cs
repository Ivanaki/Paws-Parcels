using Messages;
using Paws_Parcels.Scripts.Saves.States.EditorSettings;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Services;
using UnityEngine;

namespace Paws_Parcels.Scripts.Car_Player
{
    public class SitInCarService
    {
        private readonly MessageSystem _messageSystem;
        private readonly GameProxy _gameProxy;
        private readonly EditorSettingsSO _editorSettingsSo;
        
        private Rigidbody _carRigidbody;
        
        public SitInCarService(GameStateProvider gameStateProvider, EditorDataService editorDataService,
            MessageSystem messageSystem)
        {
            _messageSystem = messageSystem;
            _gameProxy = gameStateProvider.GameProxy;
            _editorSettingsSo = editorDataService.EditorSettings;
        }

        public bool TrySit()
        {
            if (_gameProxy.IsPlayerInCar.Value == false && _gameProxy.IsItemInHandsNull.Value)
            {
                _messageSystem.SendCompositeMessage(this, "in_car");
                _gameProxy.IsPlayerInCar.Value = true;
                return true;
            }

            return false;
        }

        public void SetCarRigidbody(Rigidbody carRigidbody)
        {
            _carRigidbody = carRigidbody;
        }
        
        public bool TryExit()
        {
            if (_carRigidbody != null)
            {
                if (_carRigidbody.linearVelocity.magnitude > _editorSettingsSo.MaxCarSpeedWhenPlayerCanExit)
                {
                    return false;
                }
            }
            
            if (_gameProxy.IsPlayerInCar.Value)
            {
                _messageSystem.SendCompositeMessage(this, "out_car");
                _gameProxy.IsPlayerInCar.Value = false;
                return true;
            }
            return false;
        }
    }
}