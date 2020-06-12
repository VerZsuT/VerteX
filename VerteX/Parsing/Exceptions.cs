using System;

namespace VerteX.Parsing.Exceptions
{
    /// <summary>
    /// Представляет исключения парсинга строки.
    /// </summary>
    [Serializable]
    public class LineParsingException : Exception
    {
        public LineParsingException(string code)
        {
            Console.WriteLine($"VerteX[ParsingError]: Не удалось распознать строку '{code}'.");
        }
    }
}
