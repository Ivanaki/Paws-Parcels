using System;
using Items;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Services;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class SpawnCagt : MonoBehaviour
{
    [Inject] private GameProxy savings;
    [SerializeField] private GameObject cat;
    [Inject] MessageSystem messageSystem;
    private void Start()
    {
        messageSystem.AddListenerWithManyMessages("spawn_cat").Subscribe(_ =>
        {
            savings.ItemsInWorld.Add(new ItemProxy(new ItemData()
            {
                Position = cat.transform.position, AngularVelocity = Vector3.zero,
                DestinationId = "cat", ItemTypeId = "cat", LinearVelocity = Vector3.zero, Rotation = Quaternion.identity
            }));
        });

    }
}
