using System.Collections.Generic;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Scripts.Player
{
    public class DiaryInteractable : BaseInteractable
    {
        [Header("Diary")] 
        [SerializeField] private string _diaryName = "first";
        [SerializeField] private GameObject _gameObject;
        
        
        [Inject] private GameProxy _gameProxy;
        
        public override void Interact()
        {
            if(_gameProxy.DiaryPages.Contains(_diaryName) == false)
                _gameProxy.DiaryPages.Add(_diaryName);
            _gameObject?.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}