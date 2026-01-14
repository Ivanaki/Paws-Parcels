using UnityEngine;

namespace Paws_Parcels.Scripts.Saves.States
{
    [CreateAssetMenu(fileName = "BaseSettingsSO", menuName = "Base states/New base settings SO")]
    public class BaseSettingsSO : ScriptableObject
    {
        [field: SerializeField] public SettingsState BaseSettings { get; private set; }
    }
}