using UnityEngine;

namespace Paws_Parcels.Scripts.Saves.States.EditorSettings
{
    [CreateAssetMenu(fileName = "EditorSettingsSO", menuName = "Editor settings/New editor settings SO")]
    public class EditorSettingsSO : ScriptableObject
    {
        
        [field: SerializeField, Tooltip("Letters in second")] 
        public int DialogSpeed { get; private set; }
    }
}