using System;
using R3;
using UnityEngine;

namespace Map
{
    public class MapUi : MonoBehaviour
    {
        public string MapId;
        [SerializeField] private GameObject _markerGameObject;
        
        private IDisposable _disposable;

        public void SetMarked(bool marked)
        {
            _markerGameObject.SetActive(marked);
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}