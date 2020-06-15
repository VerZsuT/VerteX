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
        protected int tabsCount = 0;

        /// <summary>
        /// Добавляет конструкцию WHILE после DO.
        /// </summary>
        public void AddEndingWhileConstruction(TokenList expression)
        {
            string operationCode = $"while ({expression});";

            code.Add(TransformOperationCode(operationCode));
        }

        /// <summary>
        /// Добавляет конструкцию WHILE с телом.
        /// </summary>
        /// <param name="expression"></param>
        public void AddWhileConstruction(TokenList expression)
        {
            string operationCode = TransformOperationCode($"while ({expression})");
            string beginBrace = TransformOperationCode("{");

            code.Add(operationCode + beginBrace);
            tabsCount++;
        }

        /// <summary>
        /// Добавляет конструкцию DO.
        /// </summary>
        public void AddDoConstruction()
        {
            string operationCode = TransformOperationCode("do");
            string beginBrace = TransformOperationCode("{");

            code.Add(operationCode + beginBrace);
            tabsCount++;
        }

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
            if (!isReassignment) variables.Add(variableName);

            code.Add(TransformOperationCode(operationCode));
        }

        /// <summary>
        /// Добавляет логическую конструкцию IF в код.
        /// </summary>
        /// <param name="expressionTokens">Логическое выражение.</param>
        public void AddIfConstruction(TokenList expressionTokens)
        {
            string operationCode = TransformOperationCode($"if ({expressionTokens})");
            string beginBrace = TransformOperationCode("{");

            code.Add(operationCode + beginBrace);
            tabsCount++;
        }

        /// <summary>
        /// Добавляет конструкцию ELSE в код.
        /// </summary>
        public void AddElseConstruction()
        {
            string operationCode = TransformOperationCode("else");
            string beginBrace = TransformOperationCode("{");

            code.Add(operationCode + beginBrace);
            tabsCount++;
        }

        /// <summary>
        /// Добавляет конец конструкции в код.
        /// </summary>
        public void AddConstructionEnd()
        {
            tabsCount--;
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
        /// Добавляет шапку функции в код.
        /// </summary>
        /// <param name="funcName">Имя функции.</param>
        /// <param name="attributes">Аттрибуты объявления.</param>
        public void AddFunctionHeader(string funcName, TokenList attributes)
        {
            if (header != "") throw new System.Exception("VerteX[ParsingError]: Нельзя обявлять функцию в другой функции.");
            List<string> _attributes = new List<string>();
            foreach (Token atr in attributes)
            {
                _attributes.Add("dynamic " + atr);
            }

            header = TransformFunctionHeader($"public static void {funcName}({string.Join(", ", _attributes)})");
            name = funcName;
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
        protected string TransformOperationCode(string code)
        {
            string defaultSpaces = "            ";
            string customSpaces = "";
            for (int i = 0; i < tabsCount; i++) customSpaces += "    ";

            return defaultSpaces + customSpaces + code + "\n";
        }

        /// <summary>
        /// Добавляет спец. символы в код шапки фунции для читабельности.
        /// </summary>
        protected static string TransformFunctionHeader(string code)
        {
            return "\n        " + code + "\n        {\n";
        }

        /// <summary>
        /// Добавляет спец. символы в код шапки класса для читабельности.
        /// </summary>
        protected static string TransformClassHeader(string code)
        {
            return "\n    " + code + "\n    {";
        }

        /// <summary>
        /// Возвращает код окончания фунции.
        /// </summary>
        /// <returns></returns>
        protected static string GetFunctionFooter()
        {
            return "        }\n";
        }

        /// <summary>
        /// Возвращает код окончания класса.
        /// </summary>
        /// <returns></returns>
        protected static string GetClassFooter()
        {
            return "    }\n";
        }
    }
}
