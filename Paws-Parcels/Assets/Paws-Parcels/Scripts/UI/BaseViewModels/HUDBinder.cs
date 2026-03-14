using System;
using R3;
using Services;
using UnityEngine;

namespace UI.HUD
{
    public class HUDBinder : MonoBehaviour
    {
        //[SerializeField] private DestinationsOnCanvasShower _destinationsOnCanvasShower;
        [SerializeField] private GameObject _compassGameObject;
        [SerializeField] private GameObject _parentAllHud;
        
        private readonly CompositeDisposable _disposables = new();
        
        public void Bind(HUDViewModel viewModel)
        {
            _disposables.Add(viewModel.IsCompassActive.Subscribe(active =>
            {
                _compassGameObject.SetActive(active);
            }));
            
            _disposables.Add(viewModel.ActiveHud.Subscribe(value =>
            {
                //спрятать весь hud
                _parentAllHud.SetActive(value);
            }));
            //_destinationsOnCanvasShower.Construct(viewModel.ObjectsToShow, viewModel.ObjectsPrefabsDict);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}