using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

namespace MLOOP2_L2_4
{
    public class LanguageDictionary
    {
        public Language InLanguage { get; set; }
        public Language OutLanguage { get; set; }
        public string Title { get; private set; }
        private Dictionary<string, List<string>> _dictionary;

        public LanguageDictionary()
        {
            _dictionary = new Dictionary<string, List<string>>();
        }

        public LanguageDictionary(Language inLanguage, Language outLanguage, string title)
        {
            InLanguage = inLanguage;
            OutLanguage = outLanguage;
            Title = title;
            _dictionary = new Dictionary<string, List<string>>();
        }

        [JsonPropertyName("Words")]
        public Dictionary<string, List<string>> SerializableDictionary
        {
            get => _dictionary;
            set => _dictionary = value ?? new Dictionary<string, List<string>>();
        }

        public void AddTranslation(string word, string translation)
        {
            word = word?.Trim().ToLower();
            translation = translation?.Trim();

            if (string.IsNullOrEmpty(word) || string.IsNullOrEmpty(translation))
                return;

            if (!_dictionary.ContainsKey(word))
            {
                _dictionary[word] = new List<string>();
            }

            if (!_dictionary[word].Contains(translation))
            {
                _dictionary[word].Add(translation);
            }
        }

        public bool ContainsWord(string word)
        {
            return _dictionary.ContainsKey(word?.Trim().ToLower());
        }

        public List<string> GetTranslations(string word)
        {
            word = word?.Trim().ToLower();
            return _dictionary.ContainsKey(word) ? new List<string>(_dictionary[word]) : new List<string>();
        }

        public List<string> GetAllWords()
        {
            return _dictionary.Keys.ToList();
        }

        public void RemoveWord(string word)
        {
            word = word?.Trim().ToLower();
            if (_dictionary.ContainsKey(word))
            {
                _dictionary.Remove(word);
            }
        }

        public void RemoveTranslation(string word, string translation)
        {
            word = word?.Trim().ToLower();
            if (_dictionary.ContainsKey(word))
            {
                _dictionary[word].Remove(translation);
                if (_dictionary[word].Count == 0)
                {
                    _dictionary.Remove(word);
                }
            }
        }

        public void ReplaceWord(string oldWord, string newWord)
        {
            oldWord = oldWord?.Trim().ToLower();
            newWord = newWord?.Trim().ToLower();

            if (_dictionary.ContainsKey(oldWord) && !string.IsNullOrEmpty(newWord))
            {
                var translations = _dictionary[oldWord];
                _dictionary.Remove(oldWord);
                _dictionary[newWord] = translations;
            }
        }

        public void ReplaceTranslation(string word, string oldTranslation, string newTranslation)
        {
            word = word?.Trim().ToLower();
            if (_dictionary.ContainsKey(word))
            {
                var translations = _dictionary[word];
                int index = translations.IndexOf(oldTranslation);
                if (index >= 0)
                {
                    translations[index] = newTranslation;
                }
            }
        }

        public void ExportWord(string word, string fileName)
        {
            word = word?.Trim().ToLower();
            if (_dictionary.ContainsKey(word))
            {
                var translations = _dictionary[word];
                var content = $"Слово: {word}\nПереклади:\n";
                foreach (var translation in translations)
                {
                    content += $"- {translation}\n";
                }
                content += $"\nЕкспортовано: {DateTime.Now}\nСловник: {Title}";

                File.WriteAllText(fileName, content, System.Text.Encoding.UTF8);
            }
        }
    }
}