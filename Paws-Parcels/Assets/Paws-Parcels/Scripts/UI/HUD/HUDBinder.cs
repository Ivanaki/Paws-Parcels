using R3;
using UnityEngine;

namespace UI.HUD
{
    public class HUDBinder : MonoBehaviour
    {
        [SerializeField] private GameObject _compassGameObject;
        
        public void Bind(HUDViewModel viewModel)
        {
            viewModel.IsCompassActive.Subscribe(active => _compassGameObject.SetActive(active));
        }
    }
}