using System;

namespace VerteX.Exceptions
{
    /// <summary>
    /// Ошибка запуска программы.
    /// </summary>
    public class RunException : Exception
    {
        public RunException(string message) : base($"VerteX[ОшибкаЗапуска]: {message}.") { }
    }
}
