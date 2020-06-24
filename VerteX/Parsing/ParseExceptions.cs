using System;

namespace VerteX.Exceptions
{
    /// <summary>
    /// Выбрасывается когда тело конструкции пустое.
    /// </summary>
    public class EmptyBodyException : ParseException
    {
        public EmptyBodyException(string message, int lineIndex) : base(message, lineIndex) { }
    }

    /// <summary>
    /// Выбрасывается когда выражение контрукции пустое.
    /// </summary>
    public class EmptyExpressionException : ParseException
    {
        public EmptyExpressionException(string message, int lineIndex) : base(message, lineIndex) { }
    }

    /// <summary>
    /// Предоставляет ошибки парсинга.
    /// </summary>
    public class ParseException : Exception
    {
        public ParseException(string message, int lineIndex) :
            base($"VerteX[ОшибкаПарсинга](строка {lineIndex}): {message}.")
        { }
    }
}
