using UnityEngine;

namespace MotorHome.Scripts
{
    //[CreateAssetMenu]
    public class LanguageSO : ScriptableObject
    {
        [field: SerializeField] public LanguageEnum Language { get; private set; }
        [field: SerializeField] public string LanguageName { get; private set; }
        
        [field: SerializeField] public DialogData[] Dialogs { get; private set; }
        [field: SerializeField] public InterfaceTextData InterfaceTextData { get; private set; }
        [field: SerializeField] public QuestData[] Quests { get; private set; }
    }
}