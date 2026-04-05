using System.Collections.Generic;
using UnityEngine;

namespace Paws_Parcels.Scripts.Car_Player.Respawn
{
    public class RespawnService
    {
        private readonly List<RespawnPoint> _points = new();

        private CarRespawnChecker _carRespawnChecker;
        private PlayerRespawnChecker _playerRespawnChecker;

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
            _carRespawnChecker?.Respawn();
            _playerRespawnChecker?.Respawn();
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
            RespawnPoint nearest = _points[0];

            foreach (var point in _points)
            {
                float dist = Vector3.Distance(position, point.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = point;
                }
            }

            respawnPosition = nearest.transform.position;
            respawnRotation = nearest.transform.rotation;
            return true;
        }
    }
}
