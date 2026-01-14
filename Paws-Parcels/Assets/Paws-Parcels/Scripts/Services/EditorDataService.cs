using Paws_Parcels.Scripts.Saves.States.EditorSettings;
using UnityEngine;

namespace Services
{
    public class EditorDataService
    {
        public EditorSettingsSO EditorSettings { get; }

        public EditorDataService()
        {
            EditorSettings = Resources.Load<EditorSettingsSO>("Editor/EditorSettingsSO");
        }
    }
}