using System.Collections.Generic;
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
            { "UserMethods", new List<string>() },
            { "Convert", new List<string>() {"ToInt32", "ToSingle", "ToString"} },
            { "Converter", new List<string>() {"ToBoolean"} }
        };

        /// <summary>
        /// карта имён методов для первичной замены.
        /// </summary>
        public static Dictionary<string, string> namesMap = new Dictionary<string, string>()
        {
            { "печать", "Print" },
            { "ввод", "Input" },
            { "целое", "ToInt32" },
            { "дробное", "ToSingle" },
            { "булевое", "ToBoolean" },
            { "строка", "ToString" }
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
        /// Стирает хранилище кода.
        /// </summary>
        public static void Restore()
        {
            Main = new Main();
            UserMethods = new UserMethods();
            NewFunction = new NewFunction();
        }
    }
}