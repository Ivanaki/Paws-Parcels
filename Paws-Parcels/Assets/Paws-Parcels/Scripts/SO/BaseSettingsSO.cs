using UnityEngine;

namespace Paws_Parcels.Scripts.Saves.States
{
    [CreateAssetMenu(fileName = "BaseSettingsState", menuName = "Paws_parcels/Base settings state")]
    public class BaseSettingsSO : ScriptableObject
    {
        [field: SerializeField] public SettingsState BaseSettings { get; private set; }
    }
}