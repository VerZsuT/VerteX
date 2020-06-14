using VerteX.Lexing;

namespace VerteX.Parsing
{
    /// <summary>
    /// Предоставляет методы проверки токенов для определения типа команды.
    /// </summary>
    public static class Checker
    {
        /// <summary>
        /// Определяет имеет ли конструкция начало тела.
        /// </summary>
        public static bool IsBeginBody(TokenList lineTokens)
        {
            return IsShortFunctionCreation(lineTokens) || 
                   IsBeginBrace(lineTokens) ||
                   IsShortIfConstruction(lineTokens) || 
                   IsShortElseConstruction(lineTokens);
        }

        /// <summary>
        /// Определяет имеет ли конструкция конец тела.
        /// </summary>
        public static bool IsEndingBody(TokenList lineTokens)
        {
            return IsEndBrace(lineTokens) || IsShortElseConstruction(lineTokens);
        }

        /// <summary>
        /// Определяет вызов функции.
        /// </summary>
        public static bool IsFunctionCall(TokenList lineTokens)
        {
            if (lineTokens.Count < 3) return false;

            return lineTokens[0].TypeIs(TokenType.Id) &&
                   lineTokens[1].TypeIs(TokenType.BeginParenthesis) &&
                   lineTokens[-1].TypeIs(TokenType.EndParenthesis);
        }

        /// <summary>
        /// Определяет, имеет ли функция параметры при объявлении.
        /// </summary>
        public static bool IsHaveParams(TokenList lineTokens)
        {
            if (Parser.GetExpressionInParenthesis(lineTokens).Count != 0) return true;
            return false;
        }

        /// <summary>
        /// Определяет присвоение переменной.
        /// </summary>
        public static bool IsVariableSet(TokenList lineTokens)
        {
            if (lineTokens.Count >= 3)
            {
                return lineTokens[0].TypeIs(TokenType.Id) &&
                       lineTokens[1].TypeIs(TokenType.AssignOperator);
            }
            return false;
        }

        /// <summary>
        /// Определяет объявление функции в любом стиле.
        /// </summary>
        public static bool IsFunctionCreation(TokenList currentLineTokens, TokenList nextLineTokens)
        {
            return IsShortFunctionCreation(currentLineTokens) ||
                   IsLongFunctionCreation(currentLineTokens, nextLineTokens);
        }

        /// <summary>
        /// Определяет объявление функции в коротком стиле (JavaScript стиль).
        /// </summary>
        public static bool IsShortFunctionCreation(TokenList lineTokens)
        {
            if (lineTokens.Count < 5) return false;

            return lineTokens[0].TypeIs(KeywordType.Function) &&
                   lineTokens[1].TypeIs(TokenType.Id) &&
                   lineTokens[2].TypeIs(TokenType.BeginParenthesis) &&
                   lineTokens[-2].TypeIs(TokenType.EndParenthesis) &&
                   lineTokens[-1].TypeIs(TokenType.BeginBrace);
        }

        /// <summary>
        /// Определяет объявление функции в длинном стиле (C# стиль).
        /// </summary>
        public static bool IsLongFunctionCreation(TokenList currentLineTokens, TokenList nextLineTokens)
        {
            if (currentLineTokens.Count < 4 || nextLineTokens.Count > 1) return false;

            return currentLineTokens[0].TypeIs(KeywordType.Function) &&
                   currentLineTokens[1].TypeIs(TokenType.Id) &&
                   currentLineTokens[2].TypeIs(TokenType.BeginParenthesis) &&
                   currentLineTokens[-1].TypeIs(TokenType.EndParenthesis) &&
                   nextLineTokens[0].TypeIs(TokenType.BeginBrace);
        }

        /// <summary>
        /// Определяет открывающую фигурную скобку.
        /// </summary>
        public static bool IsBeginBrace(TokenList lineTokens)
        {
            if (lineTokens.Count > 1) return false;

            return lineTokens[0].TypeIs(TokenType.BeginBrace);
        }

        /// <summary>
        /// Определяет закрывающую фигурную скобку.
        /// </summary>
        public static bool IsEndBrace(TokenList lineTokens)
        {
            if (lineTokens.Count > 1) return false;

            return lineTokens[0].TypeIs(TokenType.EndBrace);
        }

        /// <summary>
        /// Определяет конструкцию IF в любом стиле;
        /// </summary>
        public static bool IsIfConstruction(TokenList currentLineTokens, TokenList nextLineTokens)
        {
            return IsShortIfConstruction(currentLineTokens) || 
                   IsLongIfConstruction(currentLineTokens, nextLineTokens);
        }

        /// <summary>
        /// Определяет конструкцию IF в коротком стиле (JavaScript стиль).
        /// </summary>
        public static bool IsShortIfConstruction(TokenList lineTokens)
        {
            if (lineTokens.Count <= 4) return false;

            return lineTokens[0].TypeIs(KeywordType.If) &&
                   lineTokens[1].TypeIs(TokenType.BeginParenthesis) &&
                   lineTokens[-2].TypeIs(TokenType.EndParenthesis) &&
                   lineTokens[-1].TypeIs(TokenType.BeginBrace);
        }

        /// <summary>
        /// Определяет конструкцию IF в длинном стиле (C# стиль).
        /// </summary>
        public static bool IsLongIfConstruction(TokenList currentLineTokens, TokenList nextLineTokens)
        {
            if (currentLineTokens.Count <= 3) return false;

            return currentLineTokens[0].TypeIs(KeywordType.If) &&
                   currentLineTokens[1].TypeIs(TokenType.BeginParenthesis) &&
                   currentLineTokens[-1].TypeIs(TokenType.EndParenthesis) &&
                   nextLineTokens[0].TypeIs(TokenType.BeginBrace);
        }

        /// <summary>
        /// Определяет конструкцию ELSE в любом стиле.
        /// </summary>
        public static bool IsElseConstruction(TokenList currentLineTokens, TokenList nextLineTokens)
        {
            return IsShortElseConstruction(currentLineTokens) ||
                   IsLongElseConstruction(currentLineTokens, nextLineTokens);
        }

        /// <summary>
        /// Определяет конструкцию ELSE в коротком стиле (JavaScript стиль).
        /// </summary>
        public static bool IsShortElseConstruction(TokenList lineTokens)
        {
            if (lineTokens.Count == 2)
            {
                return lineTokens[0].TypeIs(KeywordType.Else) &&
                       lineTokens[1].TypeIs(TokenType.BeginBrace);
            }
            else if (lineTokens.Count == 3)
            {
                return lineTokens[0].TypeIs(TokenType.EndBrace) &&
                       lineTokens[1].TypeIs(KeywordType.Else) &&
                       lineTokens[2].TypeIs(TokenType.BeginBrace);
            }

            return false;
        }

        /// <summary>
        /// Определяет конструкцию ELSE в длинном стиле (C# стиль).
        /// </summary>
        public static bool IsLongElseConstruction(TokenList currentLineTokens, TokenList nextLineTokens)
        {
            if (nextLineTokens.Count == 1)
            {
                if (currentLineTokens.Count == 1)
                {
                    return currentLineTokens[0].TypeIs(KeywordType.Else) &&
                           nextLineTokens[0].TypeIs(TokenType.BeginBrace);
                }
                else if (currentLineTokens.Count == 2)
                {
                    return currentLineTokens[0].TypeIs(TokenType.EndBrace) &&
                           currentLineTokens[1].TypeIs(KeywordType.Else) &&
                           nextLineTokens[0].TypeIs(TokenType.BeginBrace);
                }
            }

            return false;
        }
    }
}
