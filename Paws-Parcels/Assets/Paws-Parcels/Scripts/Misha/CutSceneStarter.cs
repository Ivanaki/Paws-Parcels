using System.Linq;
using Items;
using Messages;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using Services;
using UnityEngine;
using Zenject;

public class CutSceneStarter : MonoBehaviour
{
    [SerializeField] private string cutSceneName;
    [SerializeField] private string messageNameListener;
    [SerializeField] private string nameID;
    [SerializeField] private string typeID;
    
    [Inject] private GameProxy savings;
    [Inject] MessageSystem messageSystem;
    private CompositeDisposable _disposables = new CompositeDisposable();
    
    
    private void Awake()
    {
            
        _disposables.Add( messageSystem.AddListenerWithManyMessages(messageNameListener).Subscribe(_ =>
        {
            var itemFind = savings.ItemsInWorld.FirstOrDefault(item => item.DestinationId == nameID);
            savings.ItemsInWorld.Remove(itemFind);
            savings.IsItemInHandsNull.Value = true;
            messageSystem.SendCompositeMessage(this, "start_cutscene-"+cutSceneName);
        }));
        
    }
    
    private void OnDestroy()
    {
        _disposables.Dispose(); 
    }
}
