using UnityEngine;

namespace Paws_Parcels.Scripts.Saves.States.GameState
{
    [CreateAssetMenu(fileName = "BaseGameSO", menuName = "Base states/New base game SO")]
    public class BaseGameSO : ScriptableObject
    {
        [field: SerializeField] public GameState GameState { get; private set; }
    }
}