using System;
using Items;
using Messages;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Services;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
public class SpawnFence : MonoBehaviour
{
    [Inject] private GameProxy savings;
    [SerializeField] private GameObject fence;
    [Inject] MessageSystem messageSystem;
    private CompositeDisposable _disposables = new CompositeDisposable();
    public Quaternion rot;
    private void Awake()
    {
            
        _disposables.Add( messageSystem.AddListenerWithManyMessages("spawn_fence").Subscribe(_ =>
        {
            fence.SetActive(true);
            
        }));
        
        //messageSystem.SendCompositeMessage(this, "spawn_cat");
    }

    private void OnDestroy()
    {
        _disposables.Dispose(); 
    }
}
