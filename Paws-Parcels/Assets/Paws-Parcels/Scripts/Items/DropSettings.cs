using UnityEngine;

namespace Items
{
    public class DropSettings
    {
        private PlayerDropSettings _playerDropSettings;
        
        public void SetPlayerDropSettings(PlayerDropSettings playerDropSettings)
        {
            _playerDropSettings = playerDropSettings;
        }

        public void ClearPlayerDropSettings(PlayerDropSettings playerDropSettings)
        {
            if(_playerDropSettings == playerDropSettings)
                _playerDropSettings = null;
        }
        
        public Vector3 GetDropPosition()
        {
            if(_playerDropSettings == null)
                return Vector3.zero;

            return _playerDropSettings.DropTransform.position;
        }

        public Quaternion GetDropRotation()
        {
            if(_playerDropSettings == null)
                return Quaternion.identity;
            
            return _playerDropSettings.DropTransform.rotation;
        }
        
        public Vector3 GetDropLinearVelocity()
        {
            if(_playerDropSettings == null)
                return Vector3.zero;

            return _playerDropSettings.DropTransform.TransformDirection(_playerDropSettings.LocalDropLinearVelocityVector)
                * _playerDropSettings.DropLinVelocityCoof;
        }
        
        public Vector3 GetDropAngularVelocity()
        {
            if(_playerDropSettings == null)
                return Vector3.zero;
            
            return _playerDropSettings.DropTransform.TransformDirection(_playerDropSettings.LocalDropAngularVelocity)
                   * _playerDropSettings.DropAnVelocityCoof;
        }
    }
}