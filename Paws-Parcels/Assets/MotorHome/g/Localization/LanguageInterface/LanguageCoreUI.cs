using TMPro;
using UnityEngine;

namespace MotorHome.Scripts
{
    [RequireComponent(typeof(CoreUI))]
    public class LanguageCoreUI : BaseLanguageInterface
    {
        [SerializeField] private TMP_Text _interactionText;
        [SerializeField] private TMP_Text _backText;
        [SerializeField] private TMP_Text _saveText;
        [SerializeField] private TMP_Text _loadtext;
        [SerializeField] private TMP_Text _settingsText;
        [SerializeField] private TMP_Text _mainMenuText;
        [SerializeField] private TMP_Text pauseNameText;
        
        protected override void LanguageChange(InterfaceTextData interfaceTextData)
        {
            _interactionText.text = interfaceTextData.InteractableText;
            _backText.text = interfaceTextData.PauseBack;
            _saveText.text = interfaceTextData.PauseSave;
            _loadtext.text = interfaceTextData.PauseLoad;
            _settingsText.text = interfaceTextData.PauseSettings;
            _mainMenuText.text = interfaceTextData.PauseMainMenu;
            pauseNameText.text = interfaceTextData.PauseName;
        }
    }
}