using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text;

namespace MLOOP2_L2_4
{
    class Program
    {
        private static List<LanguageDictionary> dictionaries = new List<LanguageDictionary>();
        private static string dictionariesPath = "dictionaries.json";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;
            LoadDictionaries();
            CreateTestDictionary();
            ShowMainMenu();
        }

        static void LoadDictionaries()
        {
            if (File.Exists(dictionariesPath))
            {
                try
                {
                    string json = File.ReadAllText(dictionariesPath);
                    dictionaries = JsonSerializer.Deserialize<List<LanguageDictionary>>(json) ?? new List<LanguageDictionary>();
                }
                catch
                {
                    dictionaries = new List<LanguageDictionary>();
                }
            }
        }

        static void SaveDictionaries()
        {
            string json = JsonSerializer.Serialize(dictionaries, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            File.WriteAllText(dictionariesPath, json);
        }

        static void CreateTestDictionary()
        {
            if (!dictionaries.Exists(d => d.Title == "україно-англійський"))
            {
                var testDict = new LanguageDictionary(Language.Ukrainian, Language.English, "україно-англійський");
                testDict.AddTranslation("привіт", "hello");
                testDict.AddTranslation("привіт", "hi");
                testDict.AddTranslation("дім", "house");
                testDict.AddTranslation("дім", "home");
                testDict.AddTranslation("собака", "dog");
                dictionaries.Add(testDict);
            }
        }

        static void ShowMainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n === СЛОВНИКИ ===\n");
                Console.WriteLine(" 1. Створити словник");
                Console.WriteLine(" 2. Керувати словниками");
                Console.WriteLine(" 3. Пошук перекладу");
                Console.WriteLine(" 4. Експорт слова");
                Console.WriteLine(" 0. Вийти");
                Console.Write(" Ваш вибір\n > ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        CreateDictionary();
                        break;
                    case "2":
                        ManageDictionaries();
                        break;
                    case "3":
                        SearchTranslation();
                        break;
                    case "4":
                        ExportWord();
                        break;
                    case "0":
                        SaveDictionaries();
                        return;
                    default:
                        Console.WriteLine(" Неправильний вибір!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void CreateDictionary()
        {
            Console.Clear();
            Console.WriteLine("\n === СТВОРИТИ СЛОВНИК ===");

            Console.WriteLine(" Виберіть вхідну мову:");
            Console.WriteLine(" 1. Українська");
            Console.WriteLine(" 2. Англійська");
            Console.WriteLine(" 3. Грецька");
            Console.Write(" Ваш вибір\n > ");

            Language inLang = GetLanguageChoice();
            if (inLang == (Language)(-1)) return;

            Console.WriteLine(" Виберіть вихідну мову:");
            Console.WriteLine(" 1. Українська");
            Console.WriteLine(" 2. Англійська");
            Console.WriteLine(" 3. Грецька");
            Console.Write(" Ваш вибір\n > ");

            Language outLang = GetLanguageChoice();
            if (outLang == (Language)(-1)) return;

            Console.Write(" Введіть назву словника\n > ");
            string title = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine(" Назва не може бути пустою!");
                Console.ReadKey();
                return;
            }

            if (dictionaries.Exists(d => d.Title == title))
            {
                Console.WriteLine(" Словник з такою назвою вже існує!");
                Console.ReadKey();
                return;
            }

            dictionaries.Add(new LanguageDictionary(inLang, outLang, title));
            Console.WriteLine(" Словник створено успішно!");
            Console.ReadKey();
        }

        static Language GetLanguageChoice()
        {
            string choice = Console.ReadLine();
            return choice switch
            {
                "1" => Language.Ukrainian,
                "2" => Language.English,
                "3" => Language.Greek,
                _ => (Language)(-1)
            };
        }

        static void ManageDictionaries()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n === КЕРУВАННЯ СЛОВНИКАМИ ===\n");

                if (dictionaries.Count == 0)
                {
                    Console.WriteLine(" Словники відсутні!");
                    Console.Write(" 0. Повернутися до головного меню\n > ");
                    Console.ReadKey();
                    return;
                }

                for (int i = 0; i < dictionaries.Count; i++)
                {
                    Console.WriteLine($" {i + 1}. {dictionaries[i].Title}");
                }
                Console.WriteLine(" 0. Повернутися до головного меню");
                Console.Write(" Виберіть словник\n > ");

                string choice = Console.ReadLine();
                if (choice == "0") return;

                if (int.TryParse(choice, out int index) && index > 0 && index <= dictionaries.Count)
                {
                    ManageDictionary(dictionaries[index - 1]);
                }
                else
                {
                    Console.WriteLine(" Неправильний вибір!");
                    Console.ReadKey();
                }
            }
        }

        static void ManageDictionary(LanguageDictionary dictionary)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"\n === {dictionary.Title.ToUpper()} ===\n");
                Console.WriteLine(" 1. Додати слово");
                Console.WriteLine(" 2. Замінити слово/переклад");
                Console.WriteLine(" 3. Видалити слово/переклад");
                Console.WriteLine(" 4. Показати всі слова");
                Console.WriteLine(" 0. Повернутися назад");
                Console.Write(" Ваш вибір\n > ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddWord(dictionary);
                        break;
                    case "2":
                        ReplaceWord(dictionary);
                        break;
                    case "3":
                        DeleteWord(dictionary);
                        break;
                    case "4":
                        ShowAllWords(dictionary);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine(" Неправильний вибір!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void AddWord(LanguageDictionary dictionary)
        {
            Console.Clear();
            Console.WriteLine("\n === ДОДАТИ СЛОВО ===\n");
            Console.Write(" Введіть слово\n > ");
            string word = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(word))
            {
                Console.WriteLine(" Слово не може бути пустим!");
                Console.ReadKey();
                return;
            }

            Console.Write(" Введіть переклад\n > ");
            string translation = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(translation))
            {
                Console.WriteLine(" Переклад не може бути пустим!");
                Console.ReadKey();
                return;
            }

            dictionary.AddTranslation(word, translation);
            Console.WriteLine(" Переклад додано!");
            Console.ReadKey();
        }

        static void ReplaceWord(LanguageDictionary dictionary)
        {
            Console.Clear();
            Console.WriteLine("\n === ЗАМІНИТИ СЛОВО/ПЕРЕКЛАД ===\n");
            Console.Write(" Введіть слово для заміни\n > ");
            string word = Console.ReadLine();

            if (!dictionary.ContainsWord(word))
            {
                Console.WriteLine(" Слово не знайдено!");
                Console.ReadKey();
                return;
            }

            var translations = dictionary.GetTranslations(word);
            Console.WriteLine(" Поточні переклади:");
            for (int i = 0; i < translations.Count; i++)
            {
                Console.WriteLine($" {i + 1}. {translations[i]}");
            }

            Console.WriteLine(" 1. Замінити слово");
            Console.WriteLine(" 2. Замінити переклад");
            Console.Write(" Ваш вибір\n > ");

            string choice = Console.ReadLine();
            if (choice == "1")
            {
                Console.Write(" Введіть нове слово: ");
                string newWord = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newWord))
                {
                    dictionary.ReplaceWord(word, newWord);
                    Console.WriteLine(" Слово замінено!");
                }
            }
            else if (choice == "2")
            {
                Console.Write(" Введіть номер перекладу для заміни\n > ");
                if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= translations.Count)
                {
                    Console.Write(" Введіть новий переклад\n > ");
                    string newTranslation = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newTranslation))
                    {
                        dictionary.ReplaceTranslation(word, translations[index - 1], newTranslation);
                        Console.WriteLine(" Переклад замінено!");
                    }
                }
            }
            Console.ReadKey();
        }

        static void DeleteWord(LanguageDictionary dictionary)
        {
            Console.Clear();
            Console.WriteLine("\n === ВИДАЛИТИ СЛОВО/ПЕРЕКЛАД ===\n");
            Console.Write(" Введіть слово\n > ");
            string word = Console.ReadLine();

            if (!dictionary.ContainsWord(word))
            {
                Console.WriteLine(" Слово не знайдено!");
                Console.ReadKey();
                return;
            }

            var translations = dictionary.GetTranslations(word);
            Console.WriteLine(" Поточні переклади:");
            for (int i = 0; i < translations.Count; i++)
            {
                Console.WriteLine($" {i + 1}. {translations[i]}");
            }

            Console.WriteLine($" {translations.Count + 1}. Видалити все слово");
            Console.Write(" Виберіть що видалити\n > ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                if (choice == translations.Count + 1)
                {
                    dictionary.RemoveWord(word);
                    Console.WriteLine(" Слово видалено!");
                }
                else if (choice > 0 && choice <= translations.Count)
                {
                    if (translations.Count == 1)
                    {
                        Console.WriteLine(" Не можна видалити останній переклад!");
                    }
                    else
                    {
                        dictionary.RemoveTranslation(word, translations[choice - 1]);
                        Console.WriteLine(" Переклад видалено!");
                    }
                }
            }
            Console.ReadKey();
        }

        static void ShowAllWords(LanguageDictionary dictionary)
        {
            Console.Clear();
            Console.WriteLine($"\n === ВСІ СЛОВА В {dictionary.Title.ToUpper()} ===\n");

            var allWords = dictionary.GetAllWords();
            if (allWords.Count == 0)
            {
                Console.WriteLine(" Словник порожній!");
            }
            else
            {
                foreach (var word in allWords)
                {
                    var translations = dictionary.GetTranslations(word);
                    Console.WriteLine($" {word} -> {string.Join(", ", translations)}");
                }
            }

            Console.WriteLine(" Натисніть будь-яку клавішу для продовження...");
            Console.ReadKey();
        }

        static void SearchTranslation()
        {
            Console.Clear();
            Console.WriteLine("\n === ПОШУК ПЕРЕКЛАДУ ===\n");

            if (dictionaries.Count == 0)
            {
                Console.WriteLine(" Словники відсутні!");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < dictionaries.Count; i++)
            {
                Console.WriteLine($" {i + 1}. {dictionaries[i].Title}");
            }
            Console.Write(" Виберіть словник\n > ");

            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= dictionaries.Count)
            {
                var dictionary = dictionaries[index - 1];
                Console.Write(" Введіть слово для пошуку\n > ");
                string word = Console.ReadLine();

                if (dictionary.ContainsWord(word))
                {
                    var translations = dictionary.GetTranslations(word);
                    Console.WriteLine($" Переклади для '{word}':");
                    foreach (var translation in translations)
                    {
                        Console.WriteLine($" - {translation}");
                    }
                }
                else
                {
                    Console.WriteLine(" Слово не знайдено!");
                }
            }
            else
            {
                Console.WriteLine(" Неправильний вибір!");
            }
            Console.ReadKey();
        }

        static void ExportWord()
        {
            Console.Clear();
            Console.WriteLine("\n === ЕКСПОРТ СЛОВА ===\n");

            if (dictionaries.Count == 0)
            {
                Console.WriteLine(" Словники відсутні!");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < dictionaries.Count; i++)
            {
                Console.WriteLine($" {i + 1}. {dictionaries[i].Title}");
            }
            Console.Write(" Виберіть словник\n > ");

            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= dictionaries.Count)
            {
                var dictionary = dictionaries[index - 1];
                Console.Write(" Введіть слово для експорту\n > ");
                string word = Console.ReadLine();

                if (dictionary.ContainsWord(word))
                {
                    string fileName = $"export_{word}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                    dictionary.ExportWord(word, fileName);
                    Console.WriteLine($" Слово експортовано у файл: {fileName}");
                }
                else
                {
                    Console.WriteLine(" Слово не знайдено!");
                }
            }
            else
            {
                Console.WriteLine(" Неправильний вибір!");
            }
            Console.ReadKey();
        }
    }
}