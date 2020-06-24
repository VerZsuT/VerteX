using System.Collections.Generic;

namespace VerteX.Compiling.Generators
{
    /// <summary>
    /// Представляет класс UserMethods, хранящий методы, созданные в коде.
    /// </summary>
    public class UserMethods : BaseGenerator
    {
        /// <summary>
        /// Хранит все методы, созданные в сборке.
        /// </summary>
        private readonly List<string> methods = new List<string>();

        /// <summary>
        /// Добавить метод в код.
        /// </summary>
        /// <param name="name">Имя метода.</param>
        /// <param name="code">Код в виде строки.</param>
        public void Add(string name, string code)
        {
            methods.Add(code);
        }

        /// <summary>
        /// Возвращает сгенерированный код класса UserMethods.
        /// </summary>
        public override string ToString()
        {
            if (methods.Count != 0)
            {
                string methods = string.Concat(this.methods);
                return header + methods + footer;
            }
            return "";
        }
    }
}
