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
        private static int lineIndex;

        /// <summary>
        /// Все токены, которые нужно распарсить.
        /// </summary>
        private static List<TokenList> tokens;

        /// <summary>
        /// Определяет тип команды и "дёргает" методы генераторов кода.
        /// </summary>
        /// <param name="lineTokens">Токены, созданные лексером.</param>
        /// <returns>Тип команды. Используется для отладки.</returns>
        public static void Parse(List<TokenList> codeTokens)
        {
            tokens = codeTokens;
            for (lineIndex = 0; lineIndex < codeTokens.Count; lineIndex++)
            {
                TokenList lineTokens = codeTokens[lineIndex];
                if (lineTokens.Count == 0) continue;

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
                else
                {
                    throw new LineParsingException(lineIndex + 1);
                }
            }
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

            List<TokenList> bodyTokens = GetFunctionBody();
            ParseFunctionBody(bodyTokens, methodName);
        }

        /// <summary>
        /// Получает тело функции между {}.
        /// </summary>
        private static List<TokenList> GetFunctionBody()
        {
            List<TokenList> list = new List<TokenList>();
            int beginsCount = 1;

            for (int index = lineIndex + 1; index < tokens.Count; index++)
            {
                TokenList lineTokens = tokens[index];
                if (lineTokens.Count == 0) continue;

                if (Checker.IsIfConstruction(lineTokens))
                {
                    beginsCount++;
                }
                else if (Checker.IsEndBrace(lineTokens))
                {
                    beginsCount--;
                }
                else if (Checker.IsFunctionCreation(lineTokens))
                {
                    throw new System.Exception("Нельзя объявлять новую функцию в теле другой функции!");
                }

                if (beginsCount == 0)
                {
                    lineIndex = index;
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
        /// <param name="bodyTokens"></param>
        /// <param name="functionName"></param>
        private static void ParseFunctionBody(List<TokenList> bodyTokens, string functionName)
        {
            parseMode = ParseMode.FunctionCreation;
            for (int index = 0; index < bodyTokens.Count; index++)
            {
                TokenList lineTokens = bodyTokens[index];
                if (Checker.IsFunctionCall(lineTokens))
                {
                    ParseFunctionCall(lineTokens);
                }
                else if (Checker.IsVariableSet(lineTokens))
                {
                    ParseVariableSet(lineTokens);
                }
                else if (Checker.IsEndBrace(lineTokens))
                {
                    ParseConstructionEnd();
                }
                else if (Checker.IsIfConstruction(lineTokens))
                {
                    ParseIfConstruction(lineTokens);
                }
                else
                {
                    throw new LineParsingException(lineIndex + 1, functionName);
                }
            }

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
