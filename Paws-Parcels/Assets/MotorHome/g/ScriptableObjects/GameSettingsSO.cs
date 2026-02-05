using UnityEngine;

namespace MotorHome.Scripts
{
    //[CreateAssetMenu]
    public class GameSettingsSO : ScriptableObject
    {
        [field: SerializeField] public int LettersInSecondDialogSpeed { get; private set; } = 40;
        [field: SerializeField] public float MaxPlayerRangeNearCarCoof { get; private set; } = 0.95f;
    }
}