using UnityEngine;
using UnityEngine.UI;

namespace MotorHome.Scripts
{
    public class GasUI : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private GameObject _panel;
        
        public void UpdateUI(float value)
        {
            _image.fillAmount = value;
        }

        public void Activate()
        {
            _panel.SetActive(true);
        }

        public void Deactivate()
        {
            _panel.SetActive(false);
        }
    }
}