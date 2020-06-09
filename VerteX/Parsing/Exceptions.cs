using System;

namespace VerteX.Parsing.Exceptions
{
    /// <summary>
    /// Представляет исключения парсинга строки.
    /// </summary>
    [Serializable]
    public class LineParsingException : Exception
    {
        /// <summary>
        /// Исключение парсинга строки с указанием индекса.
        /// </summary>
        public LineParsingException(int lineIndex)
        {
            Console.WriteLine($"VerteX[ParserError]: Не удалось распознать строку {lineIndex}.");
        }

        /// <summary>
        /// Исключение парсинга строки с указанием индекса и имени метода, в объявлении которого произошла ошибка.
        /// </summary>
        public LineParsingException(int lineIndex, string methodName)
        {
            Console.WriteLine($"VerteX[ParserError]: Не удалось распознать строку {lineIndex} при объявлении метода {methodName}.");
        }
    }
}
