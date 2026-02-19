using TMPro;
using UnityEngine;

namespace MotorHome.Scripts
{
    public class LanguageQuestsUI : BaseLanguageInterface
    {
        [SerializeField] private TMP_Text _selectText;
        
        protected override void LanguageChange(InterfaceTextData interfaceTextData)
        {
            _selectText.text = interfaceTextData.QuestsSelect;
        }
    }
}