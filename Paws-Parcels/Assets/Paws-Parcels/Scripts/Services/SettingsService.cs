using System.Collections.Generic;
using System.Linq;
using Paws_Parcels.Scripts.Saves.States;
using R3;
using Services;

namespace UI.Screens.Settings
{
    public class SettingsService
    {
        public class LocaleData
        {
            public string Code;
            public string Name;
        }
        
        private const string LanguageNameKey = "LanguageName";
        
        public ReadOnlyReactiveProperty<float> MasterVolume { get; }
        public ReadOnlyReactiveProperty<float> Sensitivity { get; }
        public ReadOnlyReactiveProperty<string> LanguageCode { get; }
        
        private readonly SettingsStateProvider _settingsStateProvider;
        private readonly LocalizationService _localizationService;

        public SettingsService(SettingsStateProvider settingsStateProvider, LocalizationService localizationService)
        {
            _settingsStateProvider = settingsStateProvider;
            _localizationService = localizationService;

            MasterVolume = settingsStateProvider.SettingsProxy.MasterVolume;
            Sensitivity = settingsStateProvider.SettingsProxy.Sensitivity;
            LanguageCode = settingsStateProvider.SettingsProxy.LanguageCode;
            
            LanguageCode.Skip(1).Subscribe(code =>
            {
                _localizationService.SetLanguage(code);
            });
        }

        public void SaveSettings(SettingsState settingsState)
        {
            _settingsStateProvider.SettingsProxy.Change(settingsState);
            _settingsStateProvider.SaveSettingsState();
        }
        
        public void ResetSettings()
        {
            _settingsStateProvider.ResetSettingsState();
        }

        public List<LocaleData> GetLanguages()
        {
            var localesList = _localizationService.GetAvailableLocales();
            var localeDatas = localesList.Select(locate => new LocaleData()
            {
                Code = locate.Identifier.Code,
                Name = _localizationService.GetLocalizedString(LanguageNameKey, locate, Tables.Menu)
            }).ToList();
            return localeDatas;
        }
    }
}