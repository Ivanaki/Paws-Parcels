using System;
using Items;
using Paws_Parcels.Scripts.Saves.States.GameState;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class SpawnCagt : MonoBehaviour
{
    [Inject] private GameProxy savings;
    [SerializeField] private Transform cat;

    private void Start()
    {
        savings.ItemsInWorld.Add(new ItemProxy(new ItemData(){cat.position}));
    }
}
