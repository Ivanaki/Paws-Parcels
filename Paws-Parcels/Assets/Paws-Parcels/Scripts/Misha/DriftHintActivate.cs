using System;
using Items;
using Messages;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Services;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class DriftHintActivate : MonoBehaviour
{
    [Inject] private GameProxy savings;
    [Inject] MessageSystem messageSystem;


    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            messageSystem.SendCompositeMessage(this, "show_hint:drift");
        }
            
    }
    
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            messageSystem.SendCompositeMessage(this, "hide_hint");
        }
            
    }
    
    
    private void Awake()
    {
       
    }
    
}
