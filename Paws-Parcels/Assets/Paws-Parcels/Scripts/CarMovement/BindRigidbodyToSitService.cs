using Paws_Parcels.Scripts.Car_Player;
using UnityEngine;
using Zenject;

namespace CarMovement
{
    [RequireComponent(typeof(Rigidbody))]
    public class BindRigidbodyToSitService : MonoBehaviour
    {
        [Inject] private SitInCarService _sitInCarService;
        
        private void Awake()
        {
            _sitInCarService.SetCarRigidbody(GetComponent<Rigidbody>());
        }
    }
}