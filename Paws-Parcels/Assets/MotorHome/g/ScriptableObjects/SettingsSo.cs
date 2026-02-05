using UnityEngine;

namespace MotorHome.Scripts.Saves.ScriptableObjects
{
    //[CreateAssetMenu]
    public class SettingsSo : ScriptableObject
    {
        [field: SerializeField] public SettingsState SettingsState { get; private set; }
    }
}