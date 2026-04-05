using System;
using UnityEngine;
using Zenject;

namespace Car_Player
{
    public class CarHint : MonoBehaviour
    {
        [Inject] private HintsService _hintsService;
        
        private void OnEnable()
        {
            _hintsService.SetCarHint(this);
        }
    }
}