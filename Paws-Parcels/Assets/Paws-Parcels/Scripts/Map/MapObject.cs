using UnityEngine;

namespace Map
{
    public abstract class MapObject : MonoBehaviour
    {
        public RectTransform RectTransform => _rectTransform;
        public string MapTypeId => _mapTypeId;

        [SerializeField] private string _mapTypeId;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public abstract void Construct(IShowableOnMap showableOnMap);
    }
}