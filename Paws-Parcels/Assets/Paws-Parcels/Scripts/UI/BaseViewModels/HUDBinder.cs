using System;
using R3;
using Services;
using TMPro;
using UnityEngine;

namespace UI.HUD
{
    public class HUDBinder : MonoBehaviour
    {
        //[SerializeField] private DestinationsOnCanvasShower _destinationsOnCanvasShower;
        [SerializeField] private GameObject _compassGameObject;
        [SerializeField] private GameObject _parentAllHud;
        [SerializeField] private TMP_Text _moneyText;
        
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
            
            _disposables.Add(viewModel.Money.Subscribe(value =>
            {
                _moneyText.text = value.ToString();
            }));
            //_destinationsOnCanvasShower.Construct(viewModel.ObjectsToShow, viewModel.ObjectsPrefabsDict);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}