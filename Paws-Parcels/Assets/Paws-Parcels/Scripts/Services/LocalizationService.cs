using System;
using System.Collections.Generic;
using R3;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Services
{
    public enum Tables
    {
        Menu,
        Quests,
        Locations,
        Interactables_name,
        Dialogs
    }
    
    public class LocalizationService/* : IDisposable*/
    {
        //private const string StringsTableKey = Tables.Menu.ToBinaryString();
        private const string BaseLocaleCode = "en";

        public ReadOnlyReactiveProperty<Locale> CurrentLocale => _currentLocale;
        
        public Locale BaseLocale { get; }

        private readonly ReactiveProperty<Locale> _currentLocale = new();

        public LocalizationService()
        {
            try
            {
                BaseLocale = GetLocaleOrException(BaseLocaleCode);
            }
            catch
            {
                // ignored
            }

            LocalizationSettings.SelectedLocaleChanged += ChangeLocate;
        }

        private void ChangeLocate(Locale locale)
        {
            _currentLocale.Value = locale;
        }
        
        public List<Locale> GetAvailableLocales()
        {
            return LocalizationSettings.AvailableLocales.Locales;
        }

        public Locale GetLocaleOrBase(string code)
        {
            try
            {
                return GetLocaleOrException(code);
            }
            catch
            {
                return BaseLocale;
            }
        }

        public Locale GetLocaleOrException(string code)
        {
            return LocalizationSettings.AvailableLocales.GetLocale(code);
        }
        
        public string GetCurrentLanguageStringOrDefault(string key, Tables tableName)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            try
            {
                return LocalizationSettings.StringDatabase.GetLocalizedString(tableName.ToString(), key, _currentLocale.CurrentValue);
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetLocalizedString(string key, Locale locale, Tables tableName)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }
            
            try
            {
                return LocalizationSettings.StringDatabase.GetLocalizedString(tableName.ToString(), key, locale);
            }
            catch
            {
                return string.Empty;
            }
        }

        public void SetLanguage(string code)
        {
            try
            {
                SetLanguage(GetLocaleOrException(code));
            }
            catch
            {
                SetLanguage(BaseLocale);
            }
        }

        public void SetLanguage(Locale locale)
        {
            if (locale == null)
            {
                return;
            }
            LocalizationSettings.SelectedLocale = locale;
        }

        /*public void Dispose()
        {
            _currentLocale?.Dispose();
            LocalizationSettings.SelectedLocaleChanged -= ChangeLocate;
        }*/
    }
}