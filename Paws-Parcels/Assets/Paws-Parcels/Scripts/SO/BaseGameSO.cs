using UnityEngine;

namespace Paws_Parcels.Scripts.Saves.States.GameState
{
    [CreateAssetMenu(fileName = "BaseGameState", menuName = "Paws_parcels/Base game state")]
    public class BaseGameSO : ScriptableObject
    {
        [field: SerializeField] public GameState GameState { get; private set; }
    }
}