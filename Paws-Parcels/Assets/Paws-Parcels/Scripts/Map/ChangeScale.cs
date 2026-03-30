using UnityEngine;
using UnityEngine.EventSystems;

namespace Extentions
{
    public class ChangeScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float _scaleBig = 1.5f;
        
        public void OnPointerEnter(PointerEventData eventData = null)
        {
            transform.localScale = new Vector3(_scaleBig, _scaleBig, _scaleBig);
        }

        public void OnPointerExit(PointerEventData eventData = null)
        {
            transform.localScale = Vector3.one;
            //GetComponent<Image>().alphaHitTestMinimumThreshold
        }
        
        private void OnDisable()
        {
            OnPointerExit();
        }
    }
}