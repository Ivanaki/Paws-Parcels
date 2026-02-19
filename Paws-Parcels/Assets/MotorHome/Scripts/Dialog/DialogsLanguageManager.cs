using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class DialogsLanguageManager : IInitializable
    {
        private readonly LanguageSO[] _languages;
        private readonly Dictionary<LanguageEnum, LanguageSO> _languagesDictionary = new();
        private readonly Dictionary<LanguageEnum, Dictionary<DialogEnum, PhrasesOneManData[]>> _phrasesDictionary = new();
        private readonly Dictionary<LanguageEnum, Dictionary<QuestEnum, QuestData>> _questsDictionary = new();
       //private readonly Dictionary<DialogEnum, PhrasesOneManData > _phDictionary = new();
        
        public DialogsLanguageManager(LanguageSO[] languageSO)
        {
            _languages = languageSO;
        }

        public void Initialize()
        {
            foreach (var languageSo in _languages)
            {
                _languagesDictionary.Add(languageSo.Language, languageSo);
                
                Dictionary<DialogEnum, PhrasesOneManData[]> dictionary = new();
                foreach (var languageSoDialog in languageSo.Dialogs)
                {
                    dictionary.Add(languageSoDialog.Dialog, languageSoDialog.Phrases);
                }
                _phrasesDictionary.Add(languageSo.Language, dictionary);
                
                Dictionary<QuestEnum, QuestData> dictionary2 = new();
                foreach (var languageSoQuest in languageSo.Quests)
                {
                    dictionary2.Add(languageSoQuest.Quest, languageSoQuest);
                }
                _questsDictionary.Add(languageSo.Language, dictionary2);
            }
        }

        public (bool, QuestData) GetQuestData(LanguageEnum language, QuestEnum quest)
        {
            var isDid = _questsDictionary.TryGetValue(language, out var phrases);
            if (!isDid)
            {
                Debug.LogError("Language not found: " + language);
                return (false, null);
            }
            isDid = phrases.TryGetValue(quest, out var dialogData);
            if (!isDid)
            {
                Debug.LogError("Dialog not found: " + quest);
                return (false, null);
            }
            return (true, dialogData);
        }
        
        public List<(LanguageEnum, string)> GetAllLanguages()
        {
            return _languages.Select(x => (x.Language, x.LanguageName)).ToList();
        }
        
        public InterfaceTextData GetInterfaceTextData(LanguageEnum language)
        {
            var languageSo = GetLanguageSO(language);
            return languageSo.InterfaceTextData;
        }
        
        public PhrasesOneManData[] GetDialog(LanguageEnum language, DialogEnum dialog)
        {
            var isDid = _phrasesDictionary.TryGetValue(language, out var phrases);
            if (!isDid)
            {
                Debug.LogError("Language not found: " + language);
                return null;
            }
            isDid = phrases.TryGetValue(dialog, out var dialogData);
            if (!isDid)
            {
                Debug.LogError("Dialog not found: " + dialog);
                return null;
            }
            return dialogData;
        }

        private LanguageSO GetLanguageSO(LanguageEnum language)
        {
            var isDone = _languagesDictionary.TryGetValue(language, out var languageSo);
            if (!isDone)
            {
                Debug.LogError("Language not found: " + language);
                return null;
            }
            return languageSo;
        }

        
    }
}