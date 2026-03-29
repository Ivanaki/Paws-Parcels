using System.Collections.Generic;
using UnityEngine;

namespace Paws_Parcels.Scripts.Saves.States.EditorSettings
{
    [CreateAssetMenu(fileName = "EditorSettingsState", menuName = "Paws_parcels/Editor settings state")]
    public class EditorSettingsSO : ScriptableObject
    {
        [field: SerializeField, Tooltip("Letters in second")] public int DialogSpeed { get; private set; }
        [field: SerializeField] public float MaxSensitivity { get; private set; } = 1f;
        [field: SerializeField] public float MaxSensitivityOnSlider { get; private set; } = 1f;
        [field: SerializeField] public int MaxItemsInCar { get; private set; } = 6;
        [field: SerializeField] public float MaxCarSpeedWhenPlayerCanExit { get; private set; } = 1.5f;
        
        /*public IReadOnlyList<string> Locations => _locations;
        
        [SerializeField] private List<string> _locations;*/
    }
}