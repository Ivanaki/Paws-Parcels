using UnityEngine;

namespace Paws_Parcels.Scripts.Saves.States.EditorSettings
{
    [CreateAssetMenu(fileName = "EditorSettingsState", menuName = "Paws_parcels/Editor settings state")]
    public class EditorSettingsSO : ScriptableObject
    {
        
        [field: SerializeField, Tooltip("Letters in second")] 
        public int DialogSpeed { get; private set; }
    }
}