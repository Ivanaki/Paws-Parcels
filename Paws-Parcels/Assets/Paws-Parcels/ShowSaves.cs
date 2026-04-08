using System;
using Messages;
using NaughtyAttributes;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Services;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class ShowSaves : MonoBehaviour
    {
        [Inject] private GameStateProvider _gameStateProvider;
        [Inject] private MessageSystem _messageSystem;
        

        [ReadOnly] public GameState gameState;

        private void Awake()
        {
            gameState = _gameStateProvider.GameProxy.Origin;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                _messageSystem.SendCompositeMessage(this, "On");
            }
        }
    }
}