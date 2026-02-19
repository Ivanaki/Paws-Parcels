using UnityEngine;

public class UICamera : MonoBehaviour
{
    [SerializeField] private Camera _UIcamera;
    [SerializeField] private Camera mainCamera;
    void Start()
    {
        _UIcamera.fieldOfView = 50.0181f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _UIcamera.fieldOfView  = mainCamera.fieldOfView;
    }
}
