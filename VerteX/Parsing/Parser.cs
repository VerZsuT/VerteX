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


        /// <summary>
        /// Определяет тип команды и "дёргает" методы генераторов кода.
        /// </summary>
        /// <param name="lineTokens">Токены, созданные лексером.</param>
        /// <returns>Тип команды. Используется для отладки.</returns>
        public static void Parse(List<TokenList> codeTokens)
        {
            lineIndex = 0;
            tokens = codeTokens;
            while (lineIndex < codeTokens.Count)
            {
                TokenList currentLineTokens = codeTokens[lineIndex];
                TokenList nextLineTokens = GetTokens(lineIndex + 1, codeTokens);

                Parse(currentLineTokens, nextLineTokens);
            }
        }

        private static void Parse(TokenList currentLineTokens, TokenList nextLineTokens)
        {
            if (currentLineTokens.Count == 0 && currentLineTokens.ToString().Trim() == "") return;
            if (Checker.IsBeginBrace(currentLineTokens)) return;

            if (Checker.IsFunctionCall(currentLineTokens))
            {
                ParseFunctionCall(currentLineTokens);
            }
            else if (Checker.IsVariableSet(currentLineTokens))
            {
                ParseVariableSet(currentLineTokens);
            }
            else if (Checker.IsFunctionCreation(currentLineTokens, nextLineTokens))
            {
                if (Checker.IsShortFunctionCreation(currentLineTokens))
                {
                    ParseFunctionCreation();
                }
                else if (Checker.IsLongFunctionCreation(currentLineTokens, nextLineTokens))
                {
                    ParseFunctionCreation();
                    lineIndex++;
                }
            }
            else if (Checker.IsEndBrace(currentLineTokens))
            {
                ParseConstructionEnd();
            }
            else if (Checker.IsIfConstruction(currentLineTokens, nextLineTokens))
            {
                if (Checker.IsShortIfConstruction(currentLineTokens))
                {
                    ParseIfConstruction(currentLineTokens);
                }
                else if (Checker.IsLongIfConstruction(currentLineTokens, nextLineTokens))
                {
                    ParseIfConstruction(currentLineTokens);
                    lineIndex++;
                }
            }
            else if (Checker.IsElseConstruction(currentLineTokens, nextLineTokens))
            {
                if (Checker.IsShortElseConstruction(currentLineTokens))
                {
                    ParseElseConstruction(true);
                }
                else if (Checker.IsLongElseConstruction(currentLineTokens, nextLineTokens))
                {
                    ParseElseConstruction(false);
                    lineIndex++;
                }
            }
            else
            {
                throw new LineParsingException(currentLineTokens.ToString());
            }
            lineIndex++;
        }

        /// <summary>
        /// Возвращает токены между ();
        /// </summary>
        public static TokenList GetExpressionInParenthesis(TokenList lineTokens)
        {
            TokenList list = new TokenList();
            int beginsCount = 0;

            foreach (Token token in lineTokens)
            {
                if (token.TypeIs(TokenType.BeginParenthesis))
                {
                    beginsCount++;
                }
                else if (token.TypeIs(TokenType.EndParenthesis))
                {
                    beginsCount--;
                }
                else if (beginsCount > 0)
                {
                    list.Add(token);
                }
            }

            return list;
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
        /// Парсит конструкцию ELSE.
        /// </summary>
        private static void ParseElseConstruction(bool withParenthesis)
        {
            CodeManager.GetGenerator(parseMode).AddElseConstruction(withParenthesis);
        }

        /// <summary>
        /// Парсит логическую конструкцию IF.
        /// </summary>
        private static void ParseIfConstruction(TokenList lineTokens)
        {
            TokenList expression = GetExpressionInParenthesis(lineTokens);
            CodeManager.GetGenerator(parseMode).AddIfConstruction(expression);
        }

        /// <summary>
        /// Парсит присвоение переменной.
        /// </summary>
        private static void ParseVariableSet(TokenList lineTokens)
        {
            string variableName = lineTokens[0].value;
            TokenList valueExpression = lineTokens.GetRange(2);

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
            int beginsCount = 0;
            bool isStart = true;

            for (int index = lineIndex++; index < tokens.Count; index++)
            {
                TokenList currentLineTokens = tokens[index];
                TokenList nextLineTokens = GetTokens(index + 1, tokens);

                if (currentLineTokens.Count == 0) continue;

                if (isFunctionBody && Checker.IsFunctionCreation(currentLineTokens, nextLineTokens) && !isStart)
                {
                    throw new System.Exception("Нельзя объявлять новую функцию в теле другой функции!");
                }

                if (Checker.IsEndingBody(currentLineTokens)) beginsCount--;
                if (Checker.IsBeginBody(currentLineTokens)) beginsCount++;

                if (!isStart)
                {
                    if (beginsCount == 0)
                    {
                        break;
                    }
                    else
                    {
                        list.Add(currentLineTokens);
                    }
                }

                if (beginsCount > 0) isStart = false;
            }

            return list;
        }

        /// <summary>
        /// Парсит тело функции.
        /// </summary>
        private static void ParseFunctionBody(List<TokenList> bodyTokens)
        {
            parseMode = ParseMode.FunctionCreation;
            for (int index = 0; index < bodyTokens.Count; index++)
            {
                TokenList currentLineTokens = bodyTokens[index];
                TokenList nextLineTokens = GetTokens(index + 1, bodyTokens);

                Parse(currentLineTokens, nextLineTokens);
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
            TokenList parameters = GetExpressionInParenthesis(lineTokens);
            parameters.DeleteAll(",");

            return parameters;
        }

        /// <summary>
        /// Получает аттрибуты вызова функции.
        /// </summary>
        /// <returns>Список токенов параметров, с запятыми.</returns>
        private static TokenList GetFunctionAttributes(TokenList lineTokens)
        {
            return GetExpressionInParenthesis(lineTokens);
        }

        private static TokenList GetNextTokens()
        {
            try
            {
                TokenList list = tokens[lineIndex + 1];
                return list;
            }
            catch
            {
                return new TokenList();
            }
        }

        private static TokenList GetTokens(int index, List<TokenList> tokenList)
        {
            try
            {
                return tokenList[index];
            }
            catch
            {
                return new TokenList();
            }
        }
    }
}
