using UnityEngine;
using UnityEngine.InputSystem;

namespace MotorHome.Scripts
{
    public class Hints : MonoBehaviour
    {
        [SerializeField] private GameObject _hints;
        private InputAction _hintsAction;
        
        private void Awake()
        {
            _hintsAction = InputSystem.actions.FindAction(ActionMaps.AllState + "/Hints");
            //_hints.SetActive(true);
        }
        
        private void OnEnable()
        {
            _hintsAction.performed += ToggleHints;
        }

        private void OnDisable()
        {
            _hintsAction.performed -= ToggleHints;
        }

        private bool _hintsActive = true;
        private void ToggleHints(InputAction.CallbackContext obj)
        {
            _hintsActive = !_hintsActive;
            _hints.SetActive(_hintsActive);
        }
    }
}