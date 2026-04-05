using UnityEngine;
using Zenject;

namespace Car_Player
{
    public class PlayerHint : MonoBehaviour
    {
        [Inject] private HintsService _hintsService;
        
        private void OnEnable()
        {
            _hintsService.SetPlayerHint(this);
        }
    }
}