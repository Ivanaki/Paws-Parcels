using Services;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Scripts.Player
{
    public class LocationSwitcher : MonoBehaviour
    {
        [Inject] private InteractablesLocationsService _interactablesLocationsService;

        public void SwitchLocation(string location)
        {
            _interactablesLocationsService.SwitchLocation(location);
        }
    }
}