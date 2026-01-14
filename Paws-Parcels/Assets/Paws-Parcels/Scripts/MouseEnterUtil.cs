using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Paws_Parcels.Scripts
{
    [RequireComponent(typeof(Image))]
    public class MouseEnterUtil : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public ReadOnlyReactiveProperty<bool> IsMouseEnter => _isMouseEnter;
        
        private readonly ReactiveProperty<bool> _isMouseEnter = new(false);
        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }
        
        public void OnPointerEnter(PointerEventData eventData = null)
        {
            if (_image != null)
            {
                _isMouseEnter.Value = true;
            }
        }

        public void OnPointerExit(PointerEventData eventData = null)
        {
            if (_image != null)
            {
                _isMouseEnter.Value = false;
            }
        }
    }
}