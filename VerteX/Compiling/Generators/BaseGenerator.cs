using System.Collections.Generic;
using VerteX.Lexing;

namespace VerteX.Compiling.Generators
{
    /// <summary>
    /// Родительский класс-генератор кода.
    /// </summary>
    public class BaseGenerator
    {
        protected string name = "";
        protected string header = "";
        protected string footer = "";
        protected List<string> code = new List<string>();
        protected List<string> variables = new List<string>();

        /// <summary>
        /// Добавляет присвоение переменной в код.
        /// </summary>
        /// <param name="variableName">Имя переменной.</param>
        /// <param name="variableExpression">Присваеваемое выражение.</param>
        public void AddVariableAssignment(string variableName, TokenList variableExpression)
        {
            bool isReassignment = variables.Contains(variableName);
            string prefix = !isReassignment ? "var " : "";
            string operationCode = $"{prefix}{variableName} = {variableExpression};";

            code.Add(TransformOperationCode(operationCode));
        }

        /// <summary>
        /// Добавляет логическую конструкцию IF в код.
        /// </summary>
        /// <param name="expressionTokens">Логическое выражение.</param>
        public void AddIfConstruction(TokenList expressionTokens)
        {
            string operationCode = $"if ({expressionTokens}) {{\t";
            code.Add(TransformOperationCode(operationCode));
        }

        /// <summary>
        /// Добавляет конструкцию ELSE в код.
        /// </summary>
        public void AddElseConstruction(bool withParenthesis)
        {
            string operationCode;
            if (withParenthesis) operationCode = "} else {\t";
            else operationCode = "else {\t";

            code.Add(TransformOperationCode(operationCode));
        }

        /// <summary>
        /// Добавляет конец конструкции в код.
        /// </summary>
        public void AddConstructionEnd()
        {
            code.Add(TransformOperationCode("}"));
        }

        /// <summary>
        /// Добавляет вызов функции в код.
        /// </summary>
        /// <param name="funcName">Имя функции.</param>
        /// <param name="attributes">Аттрибуты вызова.</param>
        public void AddFunctionCall(Token funcName, TokenList attributes)
        {
            string operationCode = $"{funcName}({attributes});";

            code.Add(TransformOperationCode(operationCode));
        }

        /// <summary>
        /// Добавляет вызов функции в код.
        /// </summary>
        /// <param name="funcName">Имя функции.</param>
        /// <param name="attribute">Аттрибут вызова.</param>
        public void AddFunctionCall(Token funcName, Token attribute)
        {

            string operationCode = $"{funcName}({attribute});";

            code.Add(TransformOperationCode(operationCode));
        }

        /// <summary>
        /// Добавляет вызов функции в код.
        /// </summary>
        /// <param name="funcName">Имя функции.</param>
        /// <param name="attribute">Аттрибут вызова.</param>
        public void AddFunctionCall(Token funcName, string attribute)
        {
            AddFunctionCall(funcName, new Token(TokenType.Id, attribute));
        }

        /// <summary>
        /// Добавляет вызов функции в код.
        /// </summary>
        /// <param name="funcName">Имя функции.</param>
        public void AddFunctionCall(Token funcName)
        {
            AddFunctionCall(funcName, new Token(TokenType.Id, ""));
        }

        /// <summary>
        /// Добавляет шапку функции в код.
        /// </summary>
        /// <param name="funcName">Имя функции.</param>
        /// <param name="attributes">Аттрибуты объявления.</param>
        public void AddFunctionHeader(string funcName, TokenList attributes)
        {
            List<string> _attributes = new List<string>();
            foreach (Token atr in attributes)
            {
                _attributes.Add("dynamic " + atr);
            }

            header = TransformFunctionHeader($"public static void {funcName}({string.Join(", ", _attributes)})");
            name = funcName;
        }

        /// <summary>
        /// Добавляет шапку функции в код.
        /// </summary>
        /// <param name="funcName">Имя функции.</param>
        public void AddFunctionHeader(string name)
        {
            AddFunctionHeader(name, new TokenList());
        }

        /// <summary>
        /// Возвращает сгенерированный код в виде строки.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string code = string.Concat(this.code);

            return header + code + footer;
        }

        /// <summary>
        /// Добавляет спец. символы в код для читабельности.
        /// </summary>
        protected static string TransformOperationCode(string code)
        {
            return "\t\t\t" + code + "\n";
        }

        /// <summary>
        /// Добавляет спец. символы в код шапки фунции для читабельности.
        /// </summary>
        protected static string TransformFunctionHeader(string code)
        {
            return "\n\t\t" + code + "\n\t\t{\n";
        }

        /// <summary>
        /// Добавляет спец. символы в код шапки класса для читабельности.
        /// </summary>
        protected static string TransformClassHeader(string code)
        {
            return "\n\t" + code + "\n\t{";
        }

        /// <summary>
        /// Возвращает код окончания фунции.
        /// </summary>
        /// <returns></returns>
        protected static string GetFunctionFooter()
        {
            return "\t\t}\n";
        }

        /// <summary>
        /// Возвращает код окончания класса.
        /// </summary>
        /// <returns></returns>
        protected static string GetClassFooter()
        {
            return "\t}\n";
        }
    }
}
