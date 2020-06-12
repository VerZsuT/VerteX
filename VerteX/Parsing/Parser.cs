using System.Collections.Generic;

using VerteX.Lexing;
using VerteX.Compiling;
using VerteX.Parsing.Exceptions;

namespace VerteX.Parsing
{
    /// <summary>
    /// Позволяет парсить токены, созданные лексером.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Режим парсинга токенов.
        /// </summary>
        private static ParseMode parseMode = ParseMode.Default;

        /// <summary>
        /// Индекс строки, которая парсится в текущий момент.
        /// </summary>
        private static int lineIndex = 0;

        /// <summary>
        /// Все токены, которые нужно распарсить.
        /// </summary>
        private static List<TokenList> tokens;

        public static void ParseTokens(List<TokenList> codeTokens)
        {
            tokens = codeTokens;
            while (lineIndex < codeTokens.Count)
            {
                TokenList lineTokens = codeTokens[lineIndex];
                Parse(lineTokens);
            }
        }

        /// <summary>
        /// Определяет тип команды и "дёргает" методы генераторов кода.
        /// </summary>
        /// <param name="lineTokens">Токены, созданные лексером.</param>
        /// <returns>Тип команды. Используется для отладки.</returns>
        private static void Parse(List<TokenList> codeTokens)
        {
            for (int index = 0; index < codeTokens.Count; index++)
            {
                TokenList lineTokens = codeTokens[index];
                Parse(lineTokens);
            }
        }

        private static void Parse(TokenList lineTokens)
        {
            if (lineTokens.Count == 0 && lineTokens.ToString().Trim() == "") return;

            if (Checker.IsFunctionCall(lineTokens))
            {
                ParseFunctionCall(lineTokens);
            }
            else if (Checker.IsVariableSet(lineTokens))
            {
                ParseVariableSet(lineTokens);
            }
            else if (Checker.IsFunctionCreation(lineTokens))
            {
                ParseFunctionCreation();
            }
            else if (Checker.IsEndBrace(lineTokens))
            {
                ParseConstructionEnd();
            }
            else if (Checker.IsIfConstruction(lineTokens))
            {
                ParseIfConstruction(lineTokens);
            }
            else if (Checker.IsElseConstruction(lineTokens))
            {
                ParseElseConstruction();
            }
            else
            {
                throw new LineParsingException(lineTokens.ToString());
            }
            lineIndex++;
        }

        /// <summary>
        /// Парсит конец конструкции.
        /// </summary>
        private static void ParseConstructionEnd()
        {
            CodeManager.GetGenerator(parseMode).AddConstructionEnd();
        }

        /// <summary>
        /// Парсит вызов функции.
        /// </summary>
        private static void ParseFunctionCall(TokenList lineTokens)
        {
            TokenList attributes = GetFunctionAttributes(lineTokens);
            Token methodName = lineTokens[0];

            if (attributes.Count != 0)
            {
                CodeManager.GetGenerator(parseMode).AddFunctionCall(methodName, attributes);
            }
            else
            {
                CodeManager.GetGenerator(parseMode).AddFunctionCall(methodName);
            }
        }

        private static void ParseElseConstruction()
        {
            CodeManager.GetGenerator(parseMode).AddElseConstruction();
        }

        /// <summary>
        /// Парсит логическую конструкцию IF.
        /// </summary>
        private static void ParseIfConstruction(TokenList lineTokens)
        {
            TokenList expression = lineTokens.GetRange(2, lineTokens.Count - 2);
            CodeManager.GetGenerator(parseMode).AddIfConstruction(expression);
        }

        /// <summary>
        /// Парсит присвоение переменной.
        /// </summary>
        private static void ParseVariableSet(TokenList lineTokens)
        {
            string variableName = lineTokens[0].value;
            TokenList valueExpression = lineTokens.GetRange(2, lineTokens.Count - 1);

            CodeManager.GetGenerator(parseMode).AddVariableAssignment(variableName, valueExpression);
        }

        /// <summary>
        /// Парсит объявление функции.
        /// </summary>
        private static void ParseFunctionCreation()
        {
            TokenList lineTokens = tokens[lineIndex];
            string methodName = lineTokens[1].value;

            if (Checker.IsHaveParams(lineTokens))
            {
                TokenList parameters = GetFunctionParameters(lineTokens);
                CodeManager.NewFunction.AddFunctionHeader(methodName, parameters);
            }
            else
            {
                CodeManager.NewFunction.AddFunctionHeader(methodName);
            }

            List<TokenList> bodyTokens = GetBody(true);
            ParseFunctionBody(bodyTokens);
        }

        /// <summary>
        /// Получает тело между {}.
        /// </summary>
        private static List<TokenList> GetBody(bool isFunctionBody = false)
        {
            List<TokenList> list = new List<TokenList>();
            int beginsCount = 1;

            for (int index = ++lineIndex; index < tokens.Count; index++)
            {
                TokenList lineTokens = tokens[index];
                if (lineTokens.Count == 0) continue;

                if (isFunctionBody && Checker.IsFunctionCreation(lineTokens))
                {
                    throw new System.Exception("Нельзя объявлять новую функцию в теле другой функции!");
                }

                if (Checker.IsEndingBody(lineTokens)) beginsCount--;
                if (Checker.IsBeginBody(lineTokens)) beginsCount++;

                if (beginsCount == 0)
                {
                    break;
                }
                else
                {
                    list.Add(lineTokens);
                }
            }

            return list;
        }

        /// <summary>
        /// Парсит тело функции.
        /// </summary>
        private static void ParseFunctionBody(List<TokenList> bodyTokens)
        {
            parseMode = ParseMode.FunctionCreation;
            Parse(bodyTokens);
            CodeManager.NewFunction.Create();
            parseMode = ParseMode.Default;
        }

        /// <summary>
        /// Получает параметры объявления функции.
        /// </summary>
        /// <returns>Список токенов параметров. Запятые не входят.</returns>
        private static TokenList GetFunctionParameters(TokenList lineTokens)
        {
            TokenList parameters = lineTokens.GetRange(3, lineTokens.Count - 2);
            parameters.DeleteAll(",");

            return parameters;
        }

        /// <summary>
        /// Получает аттрибуты вызова функции.
        /// </summary>
        /// <returns>Список токенов параметров, с запятыми.</returns>
        private static TokenList GetFunctionAttributes(TokenList lineTokens)
        {
            return lineTokens.GetRange(2, lineTokens.Count - 2);
        }
    }
}
