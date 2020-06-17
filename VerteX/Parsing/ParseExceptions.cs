using System;

namespace VerteX.Parsing.Exceptions
{
    /// <summary>
    /// Выбрасывается когда тело конструкции пустое.
    /// </summary>
    public class EmptyBodyException : ParseException 
    {
        public EmptyBodyException(string message) : base(message) { }
        public EmptyBodyException() { }
    }

    /// <summary>
    /// Выбрасывается когда выражение контрукции пустое.
    /// </summary>
    public class EmptyExpressionException : ParseException 
    {
        public EmptyExpressionException(string message) : base(message) { }
        public EmptyExpressionException() { }
    }

    /// <summary>
    /// Предоставляет ошибки парсинга.
    /// </summary>
    public class ParseException : Exception
    {
        public ParseException(string message)
        {
            Console.WriteLine($"VerteX[ParsingError]: {message}.");
        }

        public ParseException() { }
    }
}
