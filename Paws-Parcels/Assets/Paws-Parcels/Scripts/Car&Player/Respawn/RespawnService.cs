using System.Collections.Generic;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Services;
using UI;
using UnityEngine;

namespace Paws_Parcels.Scripts.Car_Player.Respawn
{
    public class RespawnService
    {
        private readonly List<IRespawnPoint> _points = new();
        private readonly ScreenFade _screenFade;
        private readonly SitInCarService _sitInCarService;
        private readonly GameProxy _gameProxy;
        private readonly EditorDataService _editorDataService;

        private CarRespawnChecker _carRespawnChecker;
        private PlayerRespawnChecker _playerRespawnChecker;

        public RespawnService(UIGameView uiGameView, EditorDataService editorDataService,
            SitInCarService sitInCarService, GameProxy gameProxy)
        {
            _screenFade = uiGameView.ScreenFade;
            _screenFade.SetFadeDuration(editorDataService.EditorSettings.FadeDuration);
            _sitInCarService = sitInCarService;
            _gameProxy = gameProxy;
            _editorDataService = editorDataService;
        }

        public void RegisterPoint(RespawnPoint point)
        {
            if (!_points.Contains(point))
                _points.Add(point);
        }

        public void UnregisterPoint(RespawnPoint point)
        {
            _points.Remove(point);
        }

        public void SetCarChecker(CarRespawnChecker checker)
        {
            _carRespawnChecker = checker;
        }

        public void SetPlayerChecker(PlayerRespawnChecker checker)
        {
            _playerRespawnChecker = checker;
        }

        public void Respawn()
        {
            _screenFade.FadeInOut(() =>
            {
                if (_gameProxy.IsItemInHandsNull.Value == false)
                {
                    var carPosition = _carRespawnChecker != null ? _carRespawnChecker.transform.position : Vector3.zero;
                    if (TryGetNearestPoint(carPosition, out var respawnPos, out _))
                        carPosition = respawnPos;

                    _gameProxy.ItemsInWorld.Add(new(new()
                    {
                        AngularVelocity = Vector3.zero,
                        LinearVelocity = Vector3.zero,
                        Rotation = Quaternion.identity,
                        DestinationId = _gameProxy.InHandsDestinationId,
                        ItemTypeId = _gameProxy.InHandsItemTypeId.Value,
                        Position = carPosition + new Vector3(0, _editorDataService.EditorSettings.RespawnItemDropHeight, 0)
                    }));
                    
                    _gameProxy.IsItemInHandsNull.Value = true;
                }
                
                _sitInCarService.TrySit();
                _carRespawnChecker?.Respawn();
            });
        }

        public bool TryGetNearestPoint(Vector3 position, out Vector3 respawnPosition, out Quaternion respawnRotation)
        {
            if (_points.Count == 0)
            {
                respawnPosition = default;
                respawnRotation = default;
                return false;
            }

            float minDist = float.MaxValue;
            IRespawnPoint nearest = _points[0];

            foreach (var point in _points)
            {
                float dist = Vector3.Distance(position, point.GetPosition());
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = point;
                }
            }

            respawnPosition = nearest.GetPosition();
            respawnRotation = nearest.GetRotation();
            return true;
        }
    }
}
