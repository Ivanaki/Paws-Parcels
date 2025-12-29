using UnityEngine;

namespace MotorHome.Scripts
{
    public class UIRootView : MonoBehaviour
    {
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private Transform _uiSceneContainer;
        
        /*[SerializeField] private SettingsUI _settingsUI;
        [SerializeField] private MessageGameUI _messageGameUI;
        [SerializeField] private LanguageSettingsUI _settingsinterface;
        
        public SettingsUI SettingsUI => _settingsUI;
        public MessageGameUI MessageGameUI => _messageGameUI;
        public LanguageSettingsUI SettingInterface => _settingsinterface;*/

        public void ShowLoadingScreen()
        {
            _loadingScreen.SetActive(true);
        }
        
        public void HideLoadingScreen()
        {
            _loadingScreen.SetActive(false);
        }

        public void AttachSceneUI(GameObject sceneUI)
        {
            ClearSceneUI();
            
            sceneUI.transform.SetParent(_uiSceneContainer, false);
        }

        private void ClearSceneUI()
        {
            var childCount = _uiSceneContainer.childCount;
            for (var i = 0; i < childCount; i++)
            {
                Destroy(_uiSceneContainer.GetChild(i).gameObject);
            }
        }
    }
}