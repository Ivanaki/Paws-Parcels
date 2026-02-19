using UnityEngine;

namespace MotorHome.Scripts
{
    [System.Serializable]
    public class InterfaceTextData
    {
        [Header("All game")]
        [field: SerializeField] public string GameName { get; private set; }
        
        [Header("Core")]
        [field: SerializeField] public string InteractableText { get; private set; }
        
        [Header("Pause interface")]
        [field: SerializeField] public string PauseBack { get; private set; }
        [field: SerializeField] public string PauseSave { get; private set; }
        [field: SerializeField] public string PauseLoad { get; private set; }
        [field: SerializeField] public string PauseSettings { get; private set; }
        [field: SerializeField] public string PauseMainMenu { get; private set; }
        [field: SerializeField] public string PauseName { get; private set; }
        
        [Header("MainMenu interface")]
        [field: SerializeField] public string MainMenuLoad { get; private set; }
        [field: SerializeField] public string MainMenuNewGame { get; private set; }
        [field: SerializeField] public string MainMenuSettings { get; private set; }
        [field: SerializeField] public string MainMenuExit { get; private set; }
        
        [Header("Settings")]
        [field: SerializeField] public string SettingsSaveChange { get; private set; }
        [field: SerializeField] public string SettingsSave { get; private set; }
        [field: SerializeField] public string SettingsDontSave { get; private set; }
        [field: SerializeField] public string SettingsReset { get; private set; }
        [field: SerializeField] public string SettingsName { get; private set; }
        
        [Header("Settings")]
        [field: SerializeField] public string LoadMainName { get; private set; }
        [field: SerializeField] public string LoadLoad { get; private set; }
        [field: SerializeField] public string LoadDelete { get; private set; }
        
        [Header("Quests")]
        [field: SerializeField] public string QuestsSelect { get; private set; }
        
        [Header("CoreCanvas")]
        [field: SerializeField, TextArea(2,4)] public string CoreCanvasHints { get; private set; }
        [field: SerializeField, TextArea(2,4)] public string CoreCanvasEndDemo { get; private set; }
        [field: SerializeField] public string CoreCanvasReturn { get; private set; }
        [field: SerializeField] public string CoreCanvasExit { get; private set; }
    }
}