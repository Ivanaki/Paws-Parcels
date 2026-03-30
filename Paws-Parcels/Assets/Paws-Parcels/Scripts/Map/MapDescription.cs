using R3;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Map
{
    public class MapDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        
        private readonly ReactiveProperty<bool> _isActive = new(false);
        
        public void OnPointerEnter(PointerEventData eventData = null)
        {
            _isActive.Value = true;
        }

        public void OnPointerExit(PointerEventData eventData = null)
        {
            _isActive.Value = false;
        }
        
        private void OnDisable()
        {
            OnPointerExit();
        }
    }
}