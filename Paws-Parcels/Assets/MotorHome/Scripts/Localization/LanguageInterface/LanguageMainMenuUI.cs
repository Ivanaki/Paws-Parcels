using TMPro;
using UnityEngine;

namespace MotorHome.Scripts
{
    public class LanguageMainMenuUI : BaseLanguageInterface
    {
        [SerializeField] private TMP_Text _loadText;
        [SerializeField] private TMP_Text _newGameText;
        [SerializeField] private TMP_Text _settingsText;
        [SerializeField] private TMP_Text _exitText;
        
        protected override void LanguageChange(InterfaceTextData interfaceTextData)
        {
            _loadText.text = interfaceTextData.MainMenuLoad;
            _newGameText.text = interfaceTextData.MainMenuNewGame;
            _settingsText.text = interfaceTextData.MainMenuSettings;
            _exitText.text = interfaceTextData.MainMenuExit;
        }
    }
}