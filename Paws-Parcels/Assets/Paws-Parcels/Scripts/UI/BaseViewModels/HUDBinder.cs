using System;
using R3;
using UnityEngine;

namespace UI.HUD
{
    public class HUDBinder : MonoBehaviour
    {
        [SerializeField] private GameObject _compassGameObject;
        
        private readonly CompositeDisposable _disposables = new();
        
        public void Bind(HUDViewModel viewModel)
        {
            _disposables.Add(viewModel.IsCompassActive.Subscribe(active => _compassGameObject.SetActive(active)));
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}