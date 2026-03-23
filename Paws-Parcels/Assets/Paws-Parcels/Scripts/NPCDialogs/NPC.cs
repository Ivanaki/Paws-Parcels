using System;
using System.Collections.Generic;
using Extentions;
using Paws_Parcels.Scripts.Player;
using R3;
using UnityEngine;
using Yarn.Unity;

namespace Paws_Parcels.NPCDialogs
{
    public class NPC : InteractableObject
    {
        public override string DestinationId => _npcNameCode;
        public override string UiShowTypeId => _uiShowTypeId;
        public override string UIShowTextCode => _npcNameCode;
        public override bool DontShowWithItemInHands => false;

        public override IReadOnlyList<string> ActivateLocations => _activeLocations;

        [Header("Interactable Settings")]
        [SerializeField] private string _uiShowTypeId = "interact";
        [SerializeField] private List<string> _activeLocations = new(){"None"};
        
        [Header("NPC Settings")]
        [SerializeField] private string _npcNameCode;
        [SerializeField] DialogueReference _dialogue = new();
        
        public override void Interact()
        {
            
        }
    }
}