using System;

namespace VerteX.Exceptions
{
    /// <summary>
    /// Ошибка работы лексера.
    /// </summary>
    public class LexException : Exception
    {
        public LexException(string message, int lineIndex) :
            base($"VerteX[ОшибкаЛексера]({lineIndex}): {message}.")
        { }
    }
}
