using System.Collections.Generic;
using System.IO;
using VerteX.Compiling.Generators;
using VerteX.Parsing;

namespace VerteX.Compiling
{
    /// <summary>
    /// Хранит код.
    /// </summary>
    public class CodeManager
    {
        /// <summary>
        /// Генератор кода класса Main.
        /// </summary>
        public static Main Main = new Main();

        /// <summary>
        /// Генератор класса UserMethods.
        /// </summary>
        public static UserMethods UserMethods = new UserMethods();

        /// <summary>
        /// Генератор новых методов.
        /// </summary>
        public static NewFunction NewFunction = new NewFunction();

        /// <summary>
        /// Пространства имён для вторичной замены имён методов.
        /// </summary>
        public static Dictionary<string, List<string>> namespaces = new Dictionary<string, List<string>>()
        {
            { "IO", new List<string>() {"Print", "Input"} },
            { "Convert", new List<string>() {"ToInt32", "ToSingle", "ToString", "ToChar"} },
            { "Converter", new List<string>() {"ToBoolean"} }
        };

        /// <summary>
        /// карта имён методов для первичной замены.
        /// </summary>
        public static Dictionary<string, string> namesMap = new Dictionary<string, string>()
        {
            {"печать",  "Print"},
            {"ввод",    "Input"},
            {"целое",   "ToInt32"},
            {"дробное", "ToSingle"},
            {"булевое", "ToBoolean"},
            {"строка",  "ToString"},
            {"символ",  "ToChar"}
        };

        /// <summary>
        /// Преобразовывает идентификатор (добавляет префикс).
        /// </summary>
        /// <param name="id">Исходный идентификатор.</param>
        /// <returns>С префиксом.</returns>
        public static string TransformId(string id)
        {
            foreach (string nameInMap in namesMap.Keys)
            {
                if (id == nameInMap)
                {
                    id = namesMap[nameInMap];
                }
            }
            foreach (string namespaceName in namespaces.Keys)
            {
                if (namespaces[namespaceName].Contains(id))
                {
                    return $"{namespaceName}.{id}";
                }
            }

            return id;
        }

        /// <summary>
        /// Возвращает генератор кода. Зависит от режима парсинга.
        /// </summary>
        /// <param name="mode">Режим парсинга.</param>
        public static BaseGenerator GetGenerator(ParseMode mode)
        {
            if (mode == ParseMode.Default)
            {
                return Main;
            }
            else
            {
                return NewFunction;
            }
        }

        /// <summary>
        /// Обновляет словарь значениями из файла.
        /// </summary>
        public static void UpdateNamesMap(string filePath)
        {
            string data;

            if (File.Exists(filePath))
                data = File.ReadAllText(filePath);
            else return;

            foreach (string line in data.Split('\n'))
            {
                if (line != "")
                {
                    string[] names = line.Split('=');
                    string firstName = names[0].Trim();
                    string secondName = names[1].Trim();

                    if (!namesMap.ContainsKey(firstName))
                        namesMap.Add(firstName, secondName);
                }
            }
        }
    }
}