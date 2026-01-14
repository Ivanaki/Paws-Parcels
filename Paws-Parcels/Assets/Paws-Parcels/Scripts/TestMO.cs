using R3;
using UnityEngine;

namespace Paws_Parcels.Scripts
{
    public class TestMO : MonoBehaviour
    {
        [SerializeField] private MouseEnterUtil _mouseEnterUtil;
        
        private void Start()
        {
            //_mouseEnterUtil.IsMouseEnter.Subscribe(v => print(v));
            //_gameStateProvider.GameProxy.TrackedMissionID.Value = 5;
            //_gameStateProvider.SaveGameState(true);
            //var callback = _gameStateProvider.DeleteGameState(Guid.Parse(_gameStateProvider.GetAllSaveMeta()[0].SaveId));
            //var callback = _gameStateProvider.LoadGameState(Guid.NewGuid());
            //print(callback.MessageToUser);
            //print(callback.Exception.Message);
            //_gameStateProvider.LoadNewGame();
            //print(_gameStateProvider.GameProxy.TrackedMissionID.Value);
        }
    }
}