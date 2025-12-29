using TMPro;
using UnityEngine;

namespace MotorHome.Scripts
{
    public class LanguageLoadUI : BaseLanguageInterface
    {
        [SerializeField] private TMP_Text _mainNameText;
        [SerializeField] private TMP_Text _loadText;
        [SerializeField] private TMP_Text _deleteText;
        
        protected override void LanguageChange(InterfaceTextData interfaceTextData)
        {
            _mainNameText.text = interfaceTextData.LoadMainName;
            _loadText.text = interfaceTextData.LoadLoad;
            _deleteText.text = interfaceTextData.LoadDelete;
        }
    }
}