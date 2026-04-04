using System;
using Items;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Services;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class SpawnCagt : MonoBehaviour
{
    [Inject] private GameProxy savings;
    [SerializeField] private GameObject cat;
    [Inject] MessageSystem messageSystem;
    private CompositeDisposable _disposables = new CompositeDisposable();
    public Quaternion rot;
    private void Awake()
    {
            
        _disposables.Add( messageSystem.AddListenerWithManyMessages("spawn_cat").Subscribe(_ =>
            {
                cat.SetActive(false);
                savings.ItemsInWorld.Add(new ItemProxy(new ItemData()
                {
                    Position = cat.transform.position, AngularVelocity = Vector3.zero,
                    DestinationId = "cat_1", ItemTypeId = "cat", LinearVelocity = Vector3.zero,
                    Rotation = rot
                }));
            }));
        
        //messageSystem.SendCompositeMessage(this, "spawn_cat");
    }

    private void OnDestroy()
    {
        _disposables.Dispose(); 
    }
}
