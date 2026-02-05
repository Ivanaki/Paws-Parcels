using MotorHome.Scripts.Saves.Core;
using UnityEngine;

namespace MotorHome.Scripts.Saves.ScriptableObjects
{
    //[CreateAssetMenu]
    public class CoreSo : ScriptableObject
    {
        [field: SerializeField] public CoreState CoreState { get; private set; }
    }
}