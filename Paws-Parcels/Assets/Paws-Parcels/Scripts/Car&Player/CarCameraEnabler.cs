using Unity.Cinemachine;
using UnityEngine;

namespace Car_Player
{
    public class CarCameraEnabler : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera _cinemachineCamera;
        
        public void SetCamera(bool isOn)
        {
            //TODO сделать телепорт камеры сразу, а не плавное передвижение в течении 2 секунд
            _cinemachineCamera.Priority.Value = isOn ? 100 : -100;
        }
    }
}