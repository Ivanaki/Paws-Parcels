using System;
using UnityEngine;

namespace MotorHome.Scripts
{
    public class PlayerSpawn : MonoBehaviour
    {
        private bool _needApply = false;
        private Vector3 _playerSpawnPosition;
        
        public void Spawn(Vector3 position)
        {
            _needApply = true;
            _playerSpawnPosition = position;
        }
        
        private void FixedUpdate()
        {
            if (_needApply)
            {
                SpawnPlayer();
                _needApply = false;
            }
        }
        
        private void SpawnPlayer()
        {
            transform.position = _playerSpawnPosition;
        }
    }
}