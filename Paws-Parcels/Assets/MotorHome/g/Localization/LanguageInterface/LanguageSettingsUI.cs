using System;
using TMPro;
using UnityEngine;
using R3;

namespace MotorHome.Scripts
{
    public class LanguageSettingsUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _saveChangeText;
        [SerializeField] private TMP_Text _saveText1;
        [SerializeField] private TMP_Text _saveText2;
        [SerializeField] private TMP_Text _dontSaveText;
        [SerializeField] private TMP_Text _resetText;
        [SerializeField] private TMP_Text _nameText;
        
        private IDisposable _disposable;

        public void Initialize(LanguageChanger languageChanger)
        {
            _disposable?.Dispose();
            
            _disposable = languageChanger.OnLanguageChanged.Subscribe(data =>
            {
                if (data == null) return;
                LanguageChange(data);
            });
        }
        
        private void LanguageChange(InterfaceTextData interfaceTextData)
        {
            _saveChangeText.text = interfaceTextData.SettingsSaveChange;
            _saveText1.text = interfaceTextData.SettingsSave;
            _saveText2.text = interfaceTextData.SettingsSave;
            _dontSaveText.text = interfaceTextData.SettingsDontSave;
            _resetText.text = interfaceTextData.SettingsReset;
            _nameText.text = interfaceTextData.SettingsName;
        }
    }
}