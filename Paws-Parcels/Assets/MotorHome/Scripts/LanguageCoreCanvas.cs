using TMPro;
using UnityEngine;

namespace MotorHome.Scripts
{
    public class LanguageCoreCanvas : BaseLanguageInterface
    {
        [SerializeField] private TMP_Text _hintsText;
        [SerializeField] private TMP_Text _endDemoText;
        [SerializeField] private TMP_Text _returnText;
        [SerializeField] private TMP_Text _exitText;
        
        protected override void LanguageChange(InterfaceTextData interfaceTextData)
        {
            _hintsText.text = interfaceTextData.CoreCanvasHints;
            _endDemoText.text = interfaceTextData.CoreCanvasEndDemo;
            _returnText.text = interfaceTextData.CoreCanvasReturn;
            _exitText.text = interfaceTextData.CoreCanvasExit;
        }
    }
}