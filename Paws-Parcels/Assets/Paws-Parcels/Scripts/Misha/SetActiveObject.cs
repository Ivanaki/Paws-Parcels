using System;
using Items;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Services;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
public class SetActiveObject : MonoBehaviour
{
    [Inject] private GameProxy savings;
    [SerializeField] private GameObject _gameObject;
    public bool on;
    public string message;
    [Inject] MessageSystem messageSystem;
    private CompositeDisposable _disposables = new CompositeDisposable();
    private void Awake()
    {
            
        _disposables.Add( messageSystem.AddListenerWithManyMessages(message).Subscribe(_ =>
        {
            _gameObject.SetActive(on);
            
        }));
        
        //messageSystem.SendCompositeMessage(this, "spawn_cat");
    }

    private void OnDestroy()
    {
        _disposables.Dispose(); 
    }
}
