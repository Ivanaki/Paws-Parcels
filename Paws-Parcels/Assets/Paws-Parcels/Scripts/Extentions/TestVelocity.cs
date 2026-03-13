using NaughtyAttributes;
using UnityEngine;

namespace Paws_Parcels.Extentions
{
    public class TestVelocity : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform _endPoint;
        [SerializeField] private float _offsetY;
        
        [Button]
        private void StartThrow()
        {
            _rigidbody.transform.position = _startPoint.position;
            var vel = GetVelocityParabocal(_endPoint.position, _startPoint.position, _offsetY);
            _rigidbody.AddForce(vel, ForceMode.VelocityChange);
        }
        
        private Vector3 GetVelocityParabocal(Vector3 dropPointEnd, Vector3 pointStart, float heightOffset)
        {
            float gravity = Mathf.Abs(Physics.gravity.y);

            // высота апекса
            float apexY = dropPointEnd.y + heightOffset;

            // высота подъема от старта
            float heightToApex = apexY - pointStart.y;

            if (heightToApex <= 0f)
                heightToApex = 0.01f;

            // начальная вертикальная скорость
            float velocityY = Mathf.Sqrt(2f * gravity * heightToApex);

            // время подъема
            float timeUp = velocityY / gravity;

            // время падения от апекса до цели
            float heightDown = apexY - dropPointEnd.y;
            float timeDown = Mathf.Sqrt(2f * heightDown / gravity);

            float totalTime = timeUp + timeDown;

            // горизонтальное смещение
            Vector3 horizontalDelta = new Vector3(
                dropPointEnd.x - pointStart.x,
                0f,
                dropPointEnd.z - pointStart.z
            );

            // горизонтальная скорость
            Vector3 velocityXZ = horizontalDelta / totalTime;

            // итоговый вектор скорости
            Vector3 resultVelocity = new Vector3(
                velocityXZ.x,
                velocityY,
                velocityXZ.z
            );

            return resultVelocity;
        }
    }
}